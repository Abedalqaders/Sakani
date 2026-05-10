using Application.Common.Interfaces.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationAppService _notificationService;

        public NotificationsController(INotificationAppService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications([FromQuery] bool unreadOnly = false, CancellationToken ct = default)
        {
            var notifications = await _notificationService.GetMyNotificationsAsync(unreadOnly, ct);
            return Ok(notifications);
        }

        [HttpPut("{id:guid}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken ct = default)
        {
            await _notificationService.MarkAsReadAsync(id, ct);
            return NoContent();
        }

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead(CancellationToken ct = default)
        {
            await _notificationService.MarkAllAsReadAsync(ct);
            return NoContent();
        }
    }
}