using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Application.Models.Payment.An
{
	public class PaymentDetailModel
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public string UserFullname { get; set; } = string.Empty;
		public string UserEmail { get; set; } = string.Empty;
		public int Amount { get; set; }
		public string? Description { get; set; }
		public string? OrderCode { get; set; }
		public DateTime? CreatedAt { get; set; }

		/// <summary>
		/// Danh sách payment logs theo thứ tự thời gian giảm dần
		/// </summary>
		public List<PaymentLogDetailModel> PaymentLogs { get; set; } = new();
	}

	public class PaymentLogDetailModel
	{
		public int Id { get; set; }
		public int PaymentId { get; set; }
		public string? Message { get; set; }
		public DateTime? CreatedAt { get; set; }
		public string? OldStatus { get; set; }
		public string? NewStatus { get; set; }
		public string? Metadata { get; set; }
	}

	public class PaymentServicePackage
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public string UserFullname { get; set; } = string.Empty;
		public string UserEmail { get; set; } = string.Empty;
		public int Amount { get; set; }
		public string? Description { get; set; }
		public string? OrderCode { get; set; }
		public DateTime? CreatedAt { get; set; }

		/// <summary>
		/// Danh sách payment logs theo thứ tự thời gian giảm dần
		/// </summary>
		public List<PaymentLogDetailModel> PaymentLogs { get; set; } = new();

		#region trả về thêm thông tin cần thiết
		public string? Status { get; set; }
		public int? ServicePackageId { get; set; }
		public string? ServicePackageName { get; set; }
		#endregion
	}
}
