using Application.Common.Interfaces.Notification;
using Application.Dto.Notification; // Assumed namespace for your DTO
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IReadOnlyList<NotificationResponseDto>>> GetMyNotifications([FromQuery] bool unreadOnly = false, CancellationToken ct = default)
        {
            var notifications = await _notificationService.GetMyNotificationsAsync(unreadOnly, ct);
            return Ok(notifications);
        }

        [HttpPut("{id:guid}/read")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> MarkAsRead(Guid id, CancellationToken ct = default)
        {
            await _notificationService.MarkAsReadAsync(id, ct);
            return NoContent();
        }

        [HttpPut("read-all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> MarkAllAsRead(CancellationToken ct = default)
        {
            await _notificationService.MarkAllAsReadAsync(ct);
            return NoContent();
        }
    }
}