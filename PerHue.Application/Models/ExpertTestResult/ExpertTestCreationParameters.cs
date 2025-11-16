using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Application.Models.ExpertTestResult
{
	public class ExpertTestCreationParameters
	{
		public int UserId { get; set; }
		public string ImageUrl { get; set; }
		public string? HairColor { get; set; }
		public string? EyesColor { get; set; }
		public string? LipsColor { get; set; }
		public string? SkinColor { get; set; }
	}
}
