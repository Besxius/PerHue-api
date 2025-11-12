using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PerHue.Application.Models
{
	public class CreateUserModel
	{
		[Required]
		public string Email { get; set; }
		[Required]
		[DataType(DataType.Password)]
		[MinLength(8)]
		[MaxLength(20)]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$",
			ErrorMessage = "Password must be between 8 and 20 characters long, contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
		public string Password { get; set; } = null!;
		[Required(ErrorMessage = "Please confirm your password.")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		public string? Fullname { get; set; }
		[DataType(DataType.PhoneNumber)]
		public string? Phone { get; set; }
		[Required]
		[DefaultValue(false)]
		public bool Gender { get; set; }
		[DataType(DataType.Date)]
		public DateOnly? Dob { get; set; }
		public string? Profilepicture { get; set; }
	}
}
