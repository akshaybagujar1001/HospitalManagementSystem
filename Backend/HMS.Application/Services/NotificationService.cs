using HMS.Application.DTOs.Notification;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly dynamic _hubContext;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IUnitOfWork unitOfWork,
        dynamic hubContext,
        ILogger<NotificationService> logger)
    {
        _unitOfWork = unitOfWork;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendNotificationAsync(int userId, string title, string message, string type, string? relatedEntityType = null, int? relatedEntityId = null)
    {
        try
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                RelatedEntityType = relatedEntityType,
                RelatedEntityId = relatedEntityId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            // Send real-time notification via SignalR
            await _hubContext.Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", new
            {
                id = notification.Id,
                title = notification.Title,
                message = notification.Message,
                type = notification.Type,
                isRead = notification.IsRead,
                createdAt = notification.CreatedAt
            });

            _logger.LogInformation("Notification sent to user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to user {UserId}", userId);
            throw;
        }
    }

    public async Task SendNotificationToRoleAsync(string role, string title, string message, string type)
    {
        try
        {
            var users = await _unitOfWork.Users.FindAsync(u => u.Role == role && u.IsActive);
            
            foreach (var user in users)
            {
                await SendNotificationAsync(user.Id, title, message, type);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to role {Role}", role);
            throw;
        }
    }

    public async Task MarkAsReadAsync(int notificationId, int userId)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
        if (notification != null && notification.UserId == userId)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _unitOfWork.Notifications.UpdateAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadAsync(int userId)
    {
        var notifications = await _unitOfWork.Notifications.FindAsync(n => n.UserId == userId && !n.IsRead);
        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _unitOfWork.Notifications.UpdateAsync(notification);
        }
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<int> GetUnreadCountAsync(int userId)
    {
        var count = await _unitOfWork.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);
        return count;
    }
}

