namespace PerHue.Infrastructure.Utils
{
	public enum EventTypeEnum
	{
		Created,            
		StatusChanged,      
		WebhookReceived, 
		Error,              
		Info,              
		Cancelled,  
		Refunded   
	}
	public enum PaymentStatusEnum
	{
		Pending,      
		Processing,  
		Success,   
		Failed,    
		Cancelled,  
		Refunded,  
		Expired  
	}
	public enum UserSubscriptionStatusEnum
	{
		Active,       
		Inactive,   
		Pending,   
		Cancelled,
		Expired,  
		Suspended 
	}
	public enum PerHueDefaultPassword
	{
		PerHueDefaultPasswordFA25SE166
	}
	public enum TestTypeEnum
	{
		NormalTestSimpleColor,     
		NormalTestCapsulePalette,
		AiTestUploadImage,    
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
	public enum ServicePackageTypeEnum
	{
		AI,
		Expert,
		Test
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
		UserUploadedFaceImage, 
		AiGeneratedImage,
		ExpertTestImage 
	}

	public enum VerificationStatus
	{
		Pending,
		Approved,
		Denied
	}

	public enum ReportStatus
	{
		Pending,
		InProgress,
		Resolved,
		Rejected
	}

	public enum ReportType
	{
		CustomerService,
		Complaint,
		Support
	}

	public enum ExpertTestRequestStatus
	{
		Pending,
		PendingReview,
		Completed,
		Expired
	}
	public enum TestRequestStatus
	{
		Pending,
		Completed,
		Reviewing,
		Failed
	}
}
