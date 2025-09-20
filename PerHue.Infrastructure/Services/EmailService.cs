using Microsoft.Extensions.Configuration;
using MimeKit;
using PerHue.Application.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;

namespace PerHue.Infrastructure.Services;

public class EmailService
{
	private readonly IConfiguration _config;

	public EmailService(IConfiguration config)
	{
		_config = config;
	}

	public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
	{
		try
		{
			// Validate email address
			if (string.IsNullOrEmpty(toEmail))
			{
				Console.WriteLine("Error: Email address is null or empty");
				return false;
			}

			Console.WriteLine($"Recipient email: {toEmail}");
			Console.WriteLine($"Sender email: {_config["EmailSettings:SenderEmail"]}");
			Console.WriteLine($"SMTP Host: {_config["EmailSettings:SMTPHost"]}");
			Console.WriteLine($"SMTP Port: {_config["EmailSettings:SMTPPort"]}");

			var message = new MimeMessage();

			// Add more detailed error handling for the From address
			var senderEmail = _config["EmailSettings:SenderEmail"];
			if (string.IsNullOrEmpty(senderEmail))
			{
				Console.WriteLine("Error: Sender email is null or empty");
				return false;
			}

			message.From.Add(new MailboxAddress("PerHue Service", senderEmail));
			message.To.Add(new MailboxAddress("Recipient", toEmail));
			message.Subject = subject;
			var bodyBuilder = new BodyBuilder { HtmlBody = body };
			message.Body = bodyBuilder.ToMessageBody();

			using var client = new MailKit.Net.Smtp.SmtpClient();
			await client.ConnectAsync(_config["EmailSettings:SMTPHost"],
									 int.Parse(_config["EmailSettings:SMTPPort"]),
									 false);
			await client.AuthenticateAsync(_config["EmailSettings:SenderEmail"],
										  _config["EmailSettings:SenderPassword"]);
			await client.SendAsync(message);
			await client.DisconnectAsync(true);
			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Email Error: {ex.Message}");
			Console.WriteLine($"Stack Trace: {ex.StackTrace}");
			return false;
		}
	}
}
