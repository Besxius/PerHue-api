using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Application.Models
{
	public class PostModel
	{
		public int Id { get; set; }
		public string Content { get; set; } = string.Empty;
		public int Reaction { get; set; }
		public int View { get; set; }
		public DateTime Time { get; set; }
		public int UserId { get; set; }
		public int TopicId { get; set; }
	}

}
