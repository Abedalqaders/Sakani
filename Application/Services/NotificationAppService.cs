using Application.Common.Interfaces.General;
using Application.Common.Interfaces.Notification;
using Application.Common.Interfaces.User;
using Application.Dto.Notification;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class NotificationAppService : INotificationAppService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public NotificationAppService(
            INotificationRepository notificationRepository, 
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<IReadOnlyList<NotificationResponseDto>> GetMyNotificationsAsync(bool unreadOnly, CancellationToken kt = default)
        {
            var userId = _currentUserService.UserId;
            if (userId == null || userId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("User is not authenticated properly.");
            }

            var notifications = await _notificationRepository.GetUserNotificationsAsync(userId.Value, unreadOnly, kt);

            return notifications.Select(n => new NotificationResponseDto
            {
                Id = n.Id,
                UserId = n.UserId,
                SenderId = n.SenderId,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                ReferenceId = n.ReferenceId,
                IsRead = n.IsRead,
                ReadAt = n.ReadAt,
                CreatedAt = n.CreatedAt
            }).ToList();
        }

        public async Task MarkAsReadAsync(Guid id, CancellationToken kt = default)
        {
            var userId = _currentUserService.UserId;
            if (userId == null || userId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("User is not authenticated properly.");
            }

            var notification = await _notificationRepository.GetByIdAndUserAsync(id, userId.Value, kt);
            if (notification == null)
            {
                throw new KeyNotFoundException("Notification not found or access denied.");
            }

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                _notificationRepository.Update(notification);
                await _unitOfWork.SaveChangesAsync(kt);
            }
        }

        public async Task MarkAllAsReadAsync(CancellationToken kt = default)
        {
            var userId = _currentUserService.UserId;
            if (userId == null || userId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("User is not authenticated properly.");
            }

            var unreadNotifications = await _notificationRepository.GetUnreadByUserAsync(userId.Value, kt);

            if (unreadNotifications.Any())
            {
                foreach (var notification in unreadNotifications)
                {
                    notification.IsRead = true;
                    notification.ReadAt = DateTime.UtcNow;
                    _notificationRepository.Update(notification);
                }

                await _unitOfWork.SaveChangesAsync(kt);
            }
        }
    }
}