using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Application.Models.Authentication
{
	public class LoginResponseModel
	{
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
	}
}
