using CRM.Shared.Application;
using CRM.Shared.Common;
using NotificationService.Domain.Enums;

namespace NotificationService.Application.Commands;

// ── Send Notification ─────────────────────────────────────────

public sealed record SendNotificationCommand(
    Guid                 UserId,
    NotificationType     Type,
    string               Recipient,
    string               Subject,
    string               Body,
    NotificationPriority Priority = NotificationPriority.Normal) : ICommand<Result<NotificationDto>>;

// ── Send Template Notification ────────────────────────────────

public sealed record SendTemplatedNotificationCommand(
    Guid                      UserId,
    string                    TemplateName,
    string                    Recipient,
    Dictionary<string, string> TemplateData) : ICommand<Result<NotificationDto>>;

// ── Template Management ───────────────────────────────────────

public sealed record CreateTemplateCommand(
    string           Name,
    NotificationType Type,
    string           Subject,
    string           Content) : ICommand<Result<TemplateDto>>;

public sealed record UpdateTemplateCommand(
    Guid   Id,
    string Subject,
    string Content) : ICommand<Result<TemplateDto>>;

// ── DTOs ──────────────────────────────────────────────────────

public sealed record NotificationDto(
    Guid     Id,
    Guid     UserId,
    string   Type,
    string   Recipient,
    string   Subject,
    string   Status,
    DateTime CreatedAt,
    DateTime? SentAt);

public sealed record TemplateDto(
    Guid   Id,
    string Name,
    string Type,
    string Subject,
    string Content);
