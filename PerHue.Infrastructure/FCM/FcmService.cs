using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.FCM
{
	public class FcmService : IFcmService
	{
		public async Task SendNotificationAsync(string token, string title, string body, Dictionary<string, string> data)
		{
			var message = new Message()
			{
				Token = token,
				Notification = new Notification()
				{
					Title = title,
					Body = body
				},
				Data = data 
			};

			await FirebaseMessaging.DefaultInstance.SendAsync(message);
		}

		public async Task SendMulticastAsync(List<string> tokens, string title, string body, Dictionary<string, string> data)
		{
			var message = new MulticastMessage()
			{
				Tokens = tokens,
				Notification = new Notification()
				{
					Title = title,
					Body = body
				},
				Data = data
			};

			await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
		}
	}
}
