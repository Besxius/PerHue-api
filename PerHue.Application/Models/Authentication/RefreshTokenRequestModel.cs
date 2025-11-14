using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace PerHue.Application.Models.Authentication
{
	public class RefreshTokenRequestModel
	{
		[Required]
		public string AccessToken { get; set; }

		[Required]
		public string RefreshToken { get; set; }
	}
}
