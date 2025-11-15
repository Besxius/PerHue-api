using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Application.Models
{
	public class CreateTestResponseModel
	{
		public int TestRequestId { get; set; }
		public string? Note { get; set; }
		public int? Rating { get; set; }
		public string BestColor { get; set; } = null!; // Hex codes, comma-separated
		public string WorstColor { get; set; } = null!; // Hex codes, comma-separated
		public int ColorTypeId { get; set; }
	}

	public class TestResponseModel
	{
		public int Id { get; set; }
		public int TestRequestId { get; set; }
		public int ExpertId { get; set; }
		public string? Note { get; set; }
		public DateTime? CreatedDate { get; set; }
		public int? Rating { get; set; }
		public string BestColor { get; set; } = null!;
		public string WorstColor { get; set; } = null!;
		public int ColorTypeId { get; set; }
		public string ColorTypeName { get; set; } = null!;
	}
}