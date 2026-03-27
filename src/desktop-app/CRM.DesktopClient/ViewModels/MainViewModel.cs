using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CRM.DesktopClient.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private object? _currentView;

    [ObservableProperty]
    private bool _isSidebarCollapsed;

    [ObservableProperty]
    private string _userRole = "Admin"; // Mocked for now

    [ObservableProperty]
    private string _aiSearchQuery = string.Empty;

    private readonly Services.NotificationService _notifications;

    public MainViewModel()
    {
        _notifications = new Services.NotificationService("http://localhost:5000/notifications");
        _ = _notifications.StartAsync();
        NavigateToDashboard();
    }

    [RelayCommand]
    private void ToggleSidebar() => IsSidebarCollapsed = !IsSidebarCollapsed;

    [RelayCommand]
    private async Task ExecuteAiSearch()
    {
        if (string.IsNullOrWhiteSpace(AiSearchQuery)) return;
        
        // Demonstration of AI Search linking
        CurrentView = $"AI Search Results for: {AiSearchQuery}";
        // In a real app, this would call _aiService.SearchAsync(AiSearchQuery)
        
        AiSearchQuery = string.Empty;
    }

    public bool CanAccessAdminFeatures => UserRole == "Admin";
    public bool CanAccessManagerFeatures => UserRole == "Admin" || UserRole == "Manager";

    [RelayCommand]
    private void NavigateToDashboard() => CurrentView = "Dashboard View";

    [RelayCommand]
    private void NavigateToLeads()     => CurrentView = "Leads View";
    
    [RelayCommand]
    private void NavigateToAi()        => CurrentView = "AI Assistant";
}
