namespace PerHue.Infrastructure.Utils
{
	public enum EventTypeEnum
	{
		Created,            // Giao dịch mới được tạo
		StatusChanged,      // Trạng thái thanh toán thay đổi
		WebhookReceived,    // Nhận callback/webhook từ cổng thanh toán
		Error,              // Lỗi xảy ra trong quá trình thanh toán hoặc xử lý
		Info,               // Thông tin bổ sung khác (ví dụ log hệ thống)
		Cancelled,          // Giao dịch bị hủy
		Refunded            // Giao dịch đã được hoàn tiền
	}
	public enum PaymentStatusEnum
	{
		Pending,        // Đang chờ xử lý
		Processing,     // Đang xử lý thanh toán
		Success,        // Thanh toán thành công
		Failed,         // Thanh toán thất bại
		Cancelled,      // Giao dịch bị hủy
		Refunded,       // Giao dịch đã được hoàn tiền
		Expired         // Giao dịch hết hạn
	}
	public enum UserSubscriptionStatusEnum
	{
		Active,         // Đang hoạt động
		Inactive,       // Không hoạt động
		Pending,        // Chờ xử lý (ví dụ mới đăng ký, chưa thanh toán)
		Cancelled,      // Đã hủy đăng ký
		Expired,        // Đã hết hạn đăng ký
		Suspended       // Bị tạm khóa (do vi phạm hoặc lỗi thanh toán)
	}
	public enum PerHueDefaultPassword
	{
		PerHueDefaultPasswordFA25SE166
	}
	public enum TestTypeEnum
	{
		NormalTestSimpleColor,      // Kiểm tra bình thường
		NormalTestCapsulePalette,
		AiTestUploadImage,          // Kiểm tra AI
	}

	public enum ResponseTypeEnum
	{
		Normal,
		Review
	}
	public enum PhotoTypeEnum
	{
		Identity,
		Certification,
		Face
	}

	public enum TestStatus
	{
		Pending,
		Processing,
		Completed,
		Failed,
		Cancelled
	}

	public enum PictureNotes
	{
		UserUploadedFaceImage, // "User uploaded face image for AI Test
		AiGeneratedImage, // AI Generated virtual try-on image
		ExpertTestImage // Image for expert consultation
	}

}
