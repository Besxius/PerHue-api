using PerHue.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.Services
{
	public static class BuildInfoProvider
	{
		public static BuildInfo? Load()
		{
			var path = Path.Combine(AppContext.BaseDirectory, "buildinfo.json");
			if (!File.Exists(path)) return null;

			var json = File.ReadAllText(path);
			return JsonSerializer.Deserialize<BuildInfo>(json);
		}
	}
}
