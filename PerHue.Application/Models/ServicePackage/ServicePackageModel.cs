namespace PerHue.Application.Models.ServicePackage
{
	public class ServicePackageModel
	{
		public int Id { get; set; }
		public string Name { get; set; } = null!;

		public string? Description { get; set; }

		public int Price { get; set; }

		public short? Duration { get; set; }

		public short Uses { get; set; }

		public string Type { get; set; }
		public DateTime? CreatedDate { get; set; }

		public DateTime? UpdatedDate { get; set; }
	}
}
