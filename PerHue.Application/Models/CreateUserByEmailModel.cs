using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PerHue.Application.Models
{
	public class CreateUserByEmailModel
	{
		[Required]
		public string Email { get; set; }

		public string? Fullname { get; set; }

		public string? Profilepicture { get; set; }

		[DataType(DataType.PhoneNumber)]
		public string? Phone { get; set; }

		[Required]
		[DefaultValue(false)]
		public bool Gender { get; set; }

		[DataType(DataType.Date)]
		public DateOnly? Dob { get; set; }

	}
}
