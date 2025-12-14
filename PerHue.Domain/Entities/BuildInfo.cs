namespace PerHue.Domain.Entities
{
	public class BuildInfo
	{
		public DateTime BuildTime { get; set; }
		public string Commit { get; set; } = "";
		public string Branch { get; set; } = "";
		public int RunId { get; set; }

		public string ShortCommit =>
			string.IsNullOrWhiteSpace(Commit)
				? "unknown"
				: Commit.Length <= 7
					? Commit
					: Commit[..7];

		public string BuildTimeUtc =>
			BuildTime == default
				? "unknown"
				: BuildTime
					.ToUniversalTime()
					.ToString("yyyy-MM-dd HH:mm:ss 'UTC'");

		public bool IsValid => RunId > 0 && BuildTime != default;
	}
}
