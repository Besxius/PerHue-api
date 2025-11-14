namespace PerHue.Application.Models.Payment
{
	public class PayOSResponseModel
	{
		public string Code { get; set; }
		public string Desc { get; set; } = string.Empty;
		public bool Success { get; set; }
		public object Data { get; set; } = null;
		public string Signature { get; set; } = null;

	}
}
