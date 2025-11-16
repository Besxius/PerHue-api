using System.ComponentModel.DataAnnotations;

namespace PerHue.Application.Models
{
	public class AdminUpdateUserModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = null!;

		[Required]
		[MinLength(3)]
		[MaxLength(50)]
		public string Username { get; set; } = null!;

		[MaxLength(100)]
		public string? Fullname { get; set; }

		[Phone]
		public string? Phone { get; set; }

		public bool Gender { get; set; }

		public DateOnly? Dob { get; set; }

		public bool Isactive { get; set; }

		public string? Profilepicture { get; set; }

		[Required]
		public int RoleId { get; set; }

		public string? NewPassword { get; set; }
	}
}