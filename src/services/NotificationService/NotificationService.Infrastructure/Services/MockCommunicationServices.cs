using NotificationService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace NotificationService.Infrastructure.Services;

/// <summary>
/// Mock Email Service for development.
/// In production, this would use SendGrid, Amazon SES, or Mailkit.
/// </summary>
public sealed class MockEmailService : IEmailService
{
    private readonly ILogger<MockEmailService> _logger;
    public MockEmailService(ILogger<MockEmailService> logger) => _logger = logger;

    public async Task<bool> SendEmailAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        _logger.LogInformation("📧 [MOCK EMAIL] To: {To}, Subject: {Subject}", to, subject);
        await Task.Delay(200, ct); // Simulate network latency
        return true;
    }
}

/// <summary>
/// Mock SMS Service for development.
/// In production, this would use Twilio or MessageBird.
/// </summary>
public sealed class MockSmsService : ISmsService
{
    private readonly ILogger<MockSmsService> _logger;
    public MockSmsService(ILogger<MockSmsService> logger) => _logger = logger;

    public async Task<bool> SendSmsAsync(string to, string message, CancellationToken ct = default)
    {
        _logger.LogInformation("📱 [MOCK SMS] To: {To}, Content: {Content}", to, message);
        await Task.Delay(150, ct);
        return true;
    }
}
