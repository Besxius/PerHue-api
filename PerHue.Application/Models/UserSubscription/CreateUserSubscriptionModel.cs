namespace PerHue.Application.Models.UserSubscription
{
	public class CreateUserSubscriptionModel
	{
		public int UserId { get; set; }

		public int ServicePackageId { get; set; }
		public bool Status { get; set; } = default!;
	}
}
