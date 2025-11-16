namespace PerHue.Application.Models.Notification
{
	public class CreateNotificationModel
	{
		public string Title { get; set; }
		public string Content { get; set; }
		public int Receiver { get; set; }
	}
}
