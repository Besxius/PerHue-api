using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.FCM
{
	public interface IFcmService
	{
		Task SendNotificationAsync(string token, string title, string body, Dictionary<string, string> data);
		Task SendMulticastAsync(List<string> tokens, string title, string body, Dictionary<string, string> data);
	}
}
