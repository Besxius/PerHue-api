using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.SignalR.Hub;
/// <summary>
/// Represents a SignalR hub that facilitates real-time communication between connected clients.
/// </summary>
/// <remarks>
/// The <see cref="ServerHub"/> class provides methods for broadcasting messages to all connected
/// clients. It inherits from <see cref="Microsoft.AspNetCore.SignalR.Hub"/>, enabling integration with SignalR's
/// real-time messaging framework.
/// </remarks>
public sealed class ServerHub: Microsoft.AspNetCore.SignalR.Hub
{
	public async Task SendMessage(string message)
	{
		// Broadcast the message to all connected clients
		await Clients.All.SendAsync("ReceiveMessage", message);
	}

}
