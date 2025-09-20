using System;

namespace PerHue.Application.Models
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime Time { get; set; }
        public int Receiver { get; set; }
        public string ReceiverUsername { get; set; }
    }
    
    public class CreateNotificationModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int Receiver { get; set; }
    }
}