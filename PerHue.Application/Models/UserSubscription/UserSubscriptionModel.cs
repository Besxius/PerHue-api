using PerHue.Application.Models.ServicePackage;
using PerHue.Application.Models.User;

namespace PerHue.Application.Models.UserSubscription
{
	public class UserSubscriptionModel
	{
		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		public string Status { get; set; } = null!;

		public DateTime? CreateAt { get; set; }

		public DateTime? UpdateAt { get; set; }

		public int UserId { get; set; }

		public int ServicePackageId { get; set; }

		public virtual ServicePackageModel ServicePackage { get; set; } = null!;

		public virtual UserModel User { get; set; } = null!;
	}
}
