using System.ComponentModel.DataAnnotations;

namespace PerHue.Application.Models
{
	public class UpdateUserModel
	{
		public string? Fullname { get; set; }

		public string? Phone { get; set; }

		public bool Gender { get; set; }

		public DateOnly? Dob { get; set; }

		public string? Profilepicture { get; set; }
	}
}
