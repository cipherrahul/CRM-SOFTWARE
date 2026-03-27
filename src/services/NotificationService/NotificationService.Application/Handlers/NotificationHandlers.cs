using CRM.Shared.Application;
using CRM.Shared.Common;
using NotificationService.Application.Commands;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Repositories;

namespace NotificationService.Application.Handlers;

internal sealed class SendNotificationHandler : ICommandHandler<SendNotificationCommand, Result<NotificationDto>>
{
    private readonly INotificationRepository _notifications;
    private readonly IEmailService          _email;
    private readonly ISmsService            _sms;
    private readonly IUnitOfWork            _uow;

    public SendNotificationHandler(
        INotificationRepository notifications,
        IEmailService email,
        ISmsService sms,
        IUnitOfWork uow)
    {
        _notifications = notifications;
        _email         = email;
        _sms           = sms;
        _uow           = uow;
    }

    public async Task<Result<NotificationDto>> Handle(SendNotificationCommand req, CancellationToken ct)
    {
        var notification = Notification.Create(req.UserId, req.Type, req.Recipient, req.Subject, req.Body, req.Priority);
        await _notifications.AddAsync(notification, ct);
        
        notification.MarkAsSending();
        
        bool success = req.Type switch
        {
            Domain.Enums.NotificationType.Email => await _email.SendEmailAsync(req.Recipient, req.Subject, req.Body, ct),
            Domain.Enums.NotificationType.Sms   => await _sms.SendSmsAsync(req.Recipient, req.Body, ct),
            _ => false
        };

        if (success) notification.MarkAsSent();
        else         notification.MarkAsFailed("External delivery failed.");

        await _uow.SaveChangesAsync(ct);
        return notification.ToDto();
    }
}

internal sealed class SendTemplatedNotificationHandler : ICommandHandler<SendTemplatedNotificationCommand, Result<NotificationDto>>
{
    private readonly INotificationRepository _notifications;
    private readonly ITemplateRepository     _templates;
    private readonly IEmailService          _email;
    private readonly IUnitOfWork            _uow;

    public SendTemplatedNotificationHandler(
        INotificationRepository notifications,
        ITemplateRepository templates,
        IEmailService email,
        IUnitOfWork uow)
    {
        _notifications = notifications;
        _templates     = templates;
        _email         = email;
        _uow           = uow;
    }

    public async Task<Result<NotificationDto>> Handle(SendTemplatedNotificationCommand req, CancellationToken ct)
    {
        var template = await _templates.GetByNameAsync(req.TemplateName, ct);
        if (template is null) return Error.NotFound with { Code = "Template.NotFound" };

        // Simple string replacement for template variables {{key}}
        var body = template.Content;
        foreach (var (key, value) in req.TemplateData)
            body = body.Replace($"{{{{{key}}}}}", value);

        var notification = Notification.Create(req.UserId, template.Type, req.Recipient, template.Subject, body);
        await _notifications.AddAsync(notification, ct);
        
        notification.MarkAsSending();
        var success = await _email.SendEmailAsync(req.Recipient, template.Subject, body, ct);
        
        if (success) notification.MarkAsSent();
        else         notification.MarkAsFailed("Template delivery failed.");

        await _uow.SaveChangesAsync(ct);
        return notification.ToDto();
    }
}
