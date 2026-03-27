using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace CRM.DesktopClient.Services;

public class NotificationService
{
    private HubConnection _connection;

    public NotificationService(string hubUrl)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _connection.On<string, string>("ReceiveNotification", (title, message) =>
        {
            // Logic to show notification in UI
            System.Windows.MessageBox.Show($"{title}: {message}", "Real-time Update");
        });
    }

    public async Task StartAsync()
    {
        try { await _connection.StartAsync(); }
        catch { /* Handle retry or log */ }
    }
}
