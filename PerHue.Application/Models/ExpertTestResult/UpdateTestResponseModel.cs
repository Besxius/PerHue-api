using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace PerHue.Application.Models.ExpertTestResult
{
	public class UpdateTestResponseModel
	{
		[Required]
		public string BestColor { get; set; } = null!;

		[Required]
		public string WorstColor { get; set; } = null!;

		[Required]
		public int ColorTypeId { get; set; }

		public string? Note { get; set; }
	}
}