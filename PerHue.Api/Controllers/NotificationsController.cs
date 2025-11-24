using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models.Notification;
using System.Security.Claims;

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize] // Ensure the user is logged in
	public class NotificationsController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;

		public NotificationsController(IServicesProvider servicesProvider)
		{
			_servicesProvider = servicesProvider;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<NotificationModel>>> GetMyNotifications()
		{
			var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (!int.TryParse(userIdString, out var userId))
			{
				return Unauthorized("Invalid User ID in token.");
			}

			var notifications = await _servicesProvider.NotificationService.GetByReceiverAsync(userId);
			return Ok(notifications);
		}

		[HttpGet("unread")]
		public async Task<ActionResult<IEnumerable<NotificationModel>>> GetMyUnreadNotifications()
		{
			var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (!int.TryParse(userIdString, out var userId))
			{
				return Unauthorized("Invalid User ID in token.");
			}

			var notifications = await _servicesProvider.NotificationService.GetUnreadByReceiverAsync(userId);
			return Ok(notifications);
		}

		[HttpPut("{id}/read")]
		public async Task<IActionResult> MarkAsRead(int id)
		{
			var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (!int.TryParse(userIdString, out var userId))
			{
				return Unauthorized("Invalid User ID in token.");
			}

			// Optional: Verify the notification belongs to the user before marking it read
			// This logic would ideally be in the service, but for now we'll just call the service method.
			// A robust implementation would check ownership here or in the service.

			await _servicesProvider.NotificationService.MarkAsReadAsync(id);
			return Ok(new { message = "Notification marked as read." });
		}

		[HttpPut("read-all")]
		public async Task<IActionResult> MarkAllAsRead()
		{
			var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (!int.TryParse(userIdString, out var userId))
			{
				return Unauthorized("Invalid User ID in token.");
			}

			await _servicesProvider.NotificationService.MarkAllAsReadAsync(userId);
			return Ok(new { message = "All notifications marked as read." });
		}
	}
}
