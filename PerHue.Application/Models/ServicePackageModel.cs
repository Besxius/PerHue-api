namespace PerHue.Application.Models
{
	public class ServicePackageModel
	{
		public string Name { get; set; } = null!;

		public string? Description { get; set; }

		public int Price { get; set; }

		public short? Duration { get; set; }
	}
}
