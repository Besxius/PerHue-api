using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PerHue.Application.Models;
using PerHue.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.SignalR.BroadcastService;

public class AutoEmail:BackgroundService
{
	private readonly IHubContext<Hub.ServerHub> _hubContext;
	private readonly IServiceScopeFactory _scopeFactory;
	public AutoEmail(IHubContext<Hub.ServerHub> hubContext,
		IServiceScopeFactory scopeFactory)
	{
		_hubContext = hubContext;
		_scopeFactory = scopeFactory;
	}
	/// <summary>
	/// Executes the background task asynchronously, performing periodic operations until the task is canceled.
	/// </summary>
	/// <remarks>
	/// This method runs in a loop, performing operations at regular intervals. The interval is currently
	/// set to 5 seconds. The method ensures that it respects the cancellation token to allow graceful shutdown of the
	/// background task.
	/// *NOTE: You can test this service by uncommenting the email sending code and providing valid email details.
	/// </remarks>
	/// <param name="stoppingToken">A <see cref="CancellationToken"/> that is triggered when the operation should stop. The method will monitor this
	/// token and terminate execution when cancellation is requested.</param>
	/// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			//using var scope = _scopeFactory.CreateScope();
			//var _emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
			//string guid = Guid.NewGuid().ToString();
			//string email = ""; 
			//string subject = "Automated Email Notification";
			//var body = $"Email with GUID: {guid} sent at {DateTime.Now}";
			//await _emailService.SendEmailAsync(email, subject, body);
			//await _hubContext.Clients.All.SendAsync("ReceiveEmail", body, cancellationToken: stoppingToken);
			//Console.WriteLine(body);
			DateTime now = DateTime.Now;
			Console.WriteLine($"AutoEmail Scanning at : {now}");
			await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
		}
	}

}
