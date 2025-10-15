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
	/// *NOTE: This service needs further improvement
	/// </summary>
	/// <param name="stoppingToken"></param>
	/// <returns></returns>
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using var scope = _scopeFactory.CreateScope();
		var _context = scope.ServiceProvider.GetRequiredService<PerHueDbContext>();
		while (!stoppingToken.IsCancellationRequested)
		{
			var userSubcriptionList = await _context.UserSubscriptions
				.ToListAsync(cancellationToken: stoppingToken);

			List<int> expiredSubcriptions = [];
			List<int> reminderSubcriptions = [];
			List<int> users = [];
			List<string> userEmails = [];

			foreach (var item in userSubcriptionList)
			{
				if (item.EndDate.HasValue)
				{
					var daysLeft = (item.EndDate.Value.Date - DateTime.UtcNow.Date).TotalDays;

					// expired subscriptions
					if (daysLeft <= 0 && item.Status == true) // only process if still active
					{
						expiredSubcriptions.Add(item.Id);

						var user = await _context.UserAccounts
							.FirstOrDefaultAsync(u => u.Id == item.UserId, cancellationToken: stoppingToken);
						if (user != null)
						{
							users.Add(user.Id);
							if (!string.IsNullOrEmpty(user.Email))
								userEmails.Add(user.Email);
						}

						// mark subscription as expired
						item.Status = false;
						_context.UserSubscriptions.Update(item);
						await _context.SaveChangesAsync(stoppingToken);
					}
					// soon-to-expire subscriptions (<= 7 days left)
					else if (daysLeft <= 7 && daysLeft > 0)
					{
						reminderSubcriptions.Add(item.Id);
						var user = await _context.UserAccounts
							.FirstOrDefaultAsync(u => u.Id == item.UserId, cancellationToken: stoppingToken);
						if (user != null && !string.IsNullOrEmpty(user.Email))
						{
							var _emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

							string reminderSubject = "Your subscription is ending soon!";
							string reminderBody = $"Hello {user.Fullname},\n\n" +
								$"Your subscription will expire on {item.EndDate.Value:dd/MM/yyyy}. " +
								$"That’s only {daysLeft} day(s) from now!\n\n" +
								$"👉 You still have {item.RemainingUses == 0} usage(s) left in your package.\n\n" +
								$"Remember to use all of your color analysis before it ends!.";

							// send reminder email
							await _emailService.SendEmailAsync(user.Email, reminderSubject, reminderBody);

							// optional: notify via SignalR
							await _hubContext.Clients.User(user.Id.ToString())
								.SendAsync("ReceiveReminder", reminderBody, cancellationToken: stoppingToken);

							Console.WriteLine($"Reminder email sent to {user.Email} ({daysLeft} days before expiration).");
						}
					}
				}
			}

			if (expiredSubcriptions.Count > 0)
			{
				// notify via console
				Console.WriteLine($"The following Expired User Subcriptions: [{string.Join(", ", expiredSubcriptions)}]" +
					$"\br" +
					$"AND" +
					$"\br" +
					$"The following User Id: [{string.Join(", ", users)}]");

				// get required services
				var _emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

				// send email to admin
				string guid = Guid.NewGuid().ToString();
				string email = ""; // <-- fill admin email here
				string subject = "Automated Email Notification";
				var body = $"Email with GUID: {guid} sent at {DateTime.Now}" +
					$"\br" +
					$"The following Expired User Subcriptions: [{string.Join(", ", expiredSubcriptions)}]" +
					$"\br" +
					$"The following User Id: [{string.Join(", ", users)}]";

				await _emailService.SendEmailAsync(email, subject, body);
				await _hubContext.Clients.All.SendAsync("ReceiveEmail", body, cancellationToken: stoppingToken);

				// send email to users
				if (userEmails.Count > 0)
				{
					foreach (var userEmail in userEmails)
					{
						string userSubject = "Automated Email Notification";
						var userBody = "Your subscription is over.\n" +
							$"We hope you enjoyed our services.\n\n" +
						$"Please renew to continue enjoying our services.";
						await _emailService.SendEmailAsync(userEmail, userSubject, userBody);
					}
				}
			}
			else
			{
				DateTime now = DateTime.UtcNow;
				Console.WriteLine($"Scanning Expired User Subcriptions at: {now}");
			}

			// wait before scanning again
			await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
		}
	}
}
