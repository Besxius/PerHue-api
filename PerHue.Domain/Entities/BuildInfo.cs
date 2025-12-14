using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Domain.Entities
{
	public class BuildInfo
	{
		public DateTime? BuildTime { get; set; }
		public string? Commit { get; set; } = "";
		public string? Branch { get; set; } = "";
		public int RunId { get; set; }

		public string GetShortCommit()
		{
			if (string.IsNullOrWhiteSpace(Commit))
				return "unknown";

			return Commit.Length <= 7
				? Commit
				: Commit[..7];
		}
	}
}
