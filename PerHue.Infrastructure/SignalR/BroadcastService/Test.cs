using Microsoft.AspNetCore.SignalR;
using PerHue.Infrastructure.SignalR.Hub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.SignalR.BroadcastService;

public class Test
{
	private readonly IHubContext<ServerHub> _hubContext;

	public Test(IHubContext<ServerHub> hubContext)
	{
		_hubContext = hubContext;
	}

	public async Task NotifyAllAsync(string message)
	{
		await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
	}

}
