using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Domain.Entities
{
	public class BuildInfo
	{
		public DateTime BuildTime { get; set; }
		public string Commit { get; set; } = "";
		public string Branch { get; set; } = "";
		public int RunId { get; set; }
	}
}
