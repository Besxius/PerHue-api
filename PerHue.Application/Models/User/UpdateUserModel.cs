using System.ComponentModel.DataAnnotations;

namespace PerHue.Application.Models.User
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
