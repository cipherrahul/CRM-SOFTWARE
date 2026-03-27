using NotificationService.Domain.Enums;

namespace NotificationService.Application.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body, CancellationToken ct = default);
}

public interface ISmsService
{
    Task<bool> SendSmsAsync(string to, string message, CancellationToken ct = default);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
