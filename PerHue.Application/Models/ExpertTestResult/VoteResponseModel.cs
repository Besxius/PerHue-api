using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Application.Models.ExpertTestResult
{
	public class VoteResponseModel
	{
		[Required]
		public int TestRequestId { get; set; }

		[Required]
		public int VotedResponseId { get; set; }

		public string? Note { get; set; } // Optional note from the reviewing expert
	}
}
