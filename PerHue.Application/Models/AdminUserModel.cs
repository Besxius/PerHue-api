using PerHue.Application.Models.Expert;

namespace PerHue.Application.Models
{
	public class AdminUserModel
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
		public bool Isaitested { get; set; }
		public int RoleId { get; set; }
		public string RoleName { get; set; } = null!;
		public DateTime CreatedDate { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public ExpertModel? ExpertProfile { get; set; }
	}
}