using System.ComponentModel.DataAnnotations;

namespace PerHue.Application.Models
{
	public class ChangePasswordModel
	{
		public int Id { get; set; } = default!;
		[Required(ErrorMessage = "Old password is required.")]
		public string OldPassword { get; set; } = default!;

		[Required(ErrorMessage = "Password is required.")]
		[MinLength(8)]
		[MaxLength(20)]
		[DataType(DataType.Password)]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$.!%*?&]{8,20}$",
			ErrorMessage = "Password must be between 8 and 20 characters long, contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
		public string NewPassword { get; set; }

		[Required(ErrorMessage = "Please confirm your password.")]
		[DataType(DataType.Password)]
		[Compare("NewPassword", ErrorMessage = "The old password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}
}
