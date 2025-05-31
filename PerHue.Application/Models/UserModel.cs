namespace PerHue.Application.Models
{
	public class UserModel
	{
		public string Email { get; set; } = null!;

		public string Username { get; set; } = null!;

		public string? Fullname { get; set; }

		public string? Phone { get; set; }

		public bool Gender { get; set; }

		public DateOnly? Dob { get; set; }

		public bool Isactive { get; set; }

		public string? Profilepicture { get; set; }

		public bool Isaitested { get; set; }
	}
}
