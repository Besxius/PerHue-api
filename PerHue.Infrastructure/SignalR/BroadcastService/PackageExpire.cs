using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PerHue.Domain.Entities;
using PerHue.Infrastructure.Persistence;
using PerHue.Infrastructure.Repositories;
using PerHue.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.SignalR.BroadcastService;

public class PackageExpire : BackgroundService
{
	private readonly IHubContext<Hub.ServerHub> _hubContext;
	private readonly IServiceScopeFactory _scopeFactory;
	public PackageExpire(IHubContext<Hub.ServerHub> hubContext,
		IServiceScopeFactory scopeFactory)
	{
		_hubContext = hubContext;
		_scopeFactory = scopeFactory;
	}

	/// <summary>
	/// Send email to admin and user when user subscription is expired
	/// or when subscription has 7 or fewer days left before EndDate.
	/// </summary>
	/// <param name="stoppingToken"></param>
	/// <returns></returns>
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using var scope = _scopeFactory.CreateScope();
		var _context = scope.ServiceProvider.GetRequiredService<PerHueDbContext>();
		var _emailService = scope.ServiceProvider.GetRequiredService<EmailService>(); // Resolved once for efficiency

		while (!stoppingToken.IsCancellationRequested)
		{
			var userSubcriptionList = await _context.UserSubscriptions
				.ToListAsync(cancellationToken: stoppingToken);

			List<int> expiredSubcriptions = [];
			List<int> reminderSubcriptions = []; // Kept as requested
			List<int> users = [];

			foreach (var item in userSubcriptionList)
			{
				if (item.EndDate.HasValue)
				{
					var daysLeft = (item.EndDate.Value.Date - DateTime.Now.Date).TotalDays;

					// 1. HANDLE EXPIRED SUBSCRIPTIONS
					if (daysLeft <= 0 && item.Status == true) // only process if still active
					{
						expiredSubcriptions.Add(item.Id);

						var user = await _context.UserAccounts
							.FirstOrDefaultAsync(u => u.Id == item.UserId, cancellationToken: stoppingToken);

						if (user != null)
						{
							users.Add(user.Id);

							// A. Send Email with RemainingUses
							if (!string.IsNullOrEmpty(user.Email))
							{
								string userSubject = "Your Subscription has Expired";
								var userBody = $"Dear {user.Fullname},\n\n" +
									"Your subscription has unfortunately expired.\n" +
									$"You had {item.RemainingUses} remaining use(s) left in your package which have now expired.\n\n" +
									"We hope you enjoyed our services.\n" +
									"Please renew to continue enjoying our expert color analysis.";

								await _emailService.SendEmailAsync(user.Email, userSubject, userBody);
							}

							// B. Create Notification for User
							var notification = new Notification
							{
								Title = "Subscription Expired",
								Content = $"Your subscription has expired. You had {item.RemainingUses} uses remaining left in your package which have now expired.",
								Receiver = user.Id,
								ReceivedTime = DateTime.Now,
								IsRead = false,
								Type = "System",
								TestRequestId = null // Defaulting to null as it is not linked to a specific TestRequest
							};
							_context.Notifications.Add(notification);
						}

						// Mark subscription as expired
						item.Status = false;
						_context.UserSubscriptions.Update(item);

						// Save updates (Subscription status + New Notification)
						await _context.SaveChangesAsync(stoppingToken);
					}
					// 2. HANDLE SOON-TO-EXPIRE SUBSCRIPTIONS (<= 7 days left)
					else if (daysLeft <= 7 && daysLeft > 0)
					{
						reminderSubcriptions.Add(item.Id);
						var user = await _context.UserAccounts
							.FirstOrDefaultAsync(u => u.Id == item.UserId, cancellationToken: stoppingToken);

						if (user != null && !string.IsNullOrEmpty(user.Email))
						{
							string reminderSubject = "Your subscription is ending soon!";
							string reminderBody = $"Hello {user.Fullname},\n\n" +
								$"Your subscription will expire on {item.EndDate.Value:dd/MM/yyyy}. " +
								$"That’s only {daysLeft} day(s) from now!\n\n" +
								$"👉 You still have {item.RemainingUses} usage(s) left in your package.\n\n" +
								$"Remember to use all of your color analysis before it ends!.";

							// Send reminder email
							await _emailService.SendEmailAsync(user.Email, reminderSubject, reminderBody);

							// Optional: notify via SignalR
							await _hubContext.Clients.User(user.Id.ToString())
								.SendAsync("ReceiveReminder", reminderBody, cancellationToken: stoppingToken);

							Console.WriteLine($"Reminder email sent to {user.Email} ({daysLeft} days before expiration).");
						}
					}
				}
			}

			// 3. ADMIN SUMMARY NOTIFICATION
			if (expiredSubcriptions.Count > 0)
			{
				// Notify via console
				Console.WriteLine($"The following Expired User Subcriptions: [{string.Join(", ", expiredSubcriptions)}]" +
					$"\br" +
					$"AND" +
					$"\br" +
					$"The following User Id: [{string.Join(", ", users)}]");

				// Send email to admin
				string guid = Guid.NewGuid().ToString();
				string email = ""; // <-- fill admin email here

				if (!string.IsNullOrEmpty(email))
				{
					string subject = "Automated Email Notification";
					var body = $"Email with GUID: {guid} sent at {DateTime.Now}" +
						$"\br" +
						$"The following Expired User Subcriptions: [{string.Join(", ", expiredSubcriptions)}]" +
						$"\br" +
						$"The following User Id: [{string.Join(", ", users)}]";

					await _emailService.SendEmailAsync(email, subject, body);
					await _hubContext.Clients.All.SendAsync("ReceiveEmail", body, cancellationToken: stoppingToken);
				}
			}
			else
			{
				DateTime now = DateTime.Now;
				Console.WriteLine($"Scanning Expired User Subcriptions at: {now}");
			}

			// Wait before scanning again
			await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
		}
	}
}