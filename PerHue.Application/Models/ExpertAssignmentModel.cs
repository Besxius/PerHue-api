using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Application.Models
{
	public class ExpertAssignmentModel : TestRequestModel
	{
		/// <summary>
		/// The status of the request specifically for this expert (e.g., Pending, Completed, Expired)
		/// </summary>
		public string ExpertStatus { get; set; } = null!;

		/// <summary>
		/// The date this request was assigned to the expert
		/// </summary>
		public DateTime AssignmentDate { get; set; }
	}
}