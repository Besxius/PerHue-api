using PerHue.Application.Models.Role;

namespace PerHue.Application.Models.User
{
	public class UserModel
	{
		public int Id { get; set; }

		public string Email { get; set; } = null!;

		public string Username { get; set; } = null!;

		public string? Fullname { get; set; }

		public string? Phone { get; set; }

		public bool Gender { get; set; }

		public DateOnly? Dob { get; set; }

		public bool Isactive { get; set; }

		public string? Profilepicture { get; set; }

		public int RoleId { get; set; }

		public virtual RoleModel Role { get; set; } = null!;
	}
}
