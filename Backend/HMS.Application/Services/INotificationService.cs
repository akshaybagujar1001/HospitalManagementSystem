using HMS.Application.DTOs.Notification;

namespace HMS.Application.Services;

public interface INotificationService
{
    Task SendNotificationAsync(int userId, string title, string message, string type, string? relatedEntityType = null, int? relatedEntityId = null);
    Task SendNotificationToRoleAsync(string role, string title, string message, string type);
    Task MarkAsReadAsync(int notificationId, int userId);
    Task MarkAllAsReadAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);
}

