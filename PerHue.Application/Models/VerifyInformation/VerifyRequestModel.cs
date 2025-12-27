using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PerHue.Application.Models.VerifyInformation;

public class VerifyRequestModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters")]
    public string Email { get; set; } = null!;

    [StringLength(50, ErrorMessage = "Nickname cannot exceed 50 characters")]
    public string? Nickname { get; set; }

    [Required(ErrorMessage = "Specialization is required")]
    [StringLength(255, ErrorMessage = "Specialization cannot exceed 255 characters")]
    public string Specialization { get; set; } = null!;

    [Required(ErrorMessage = "Bio is required")]
    [StringLength(1000, ErrorMessage = "Bio cannot exceed 1000 characters")]
    public string Bio { get; set; } = null!;

    [Required(ErrorMessage = "Years of experience is required")]
    [Range(1, 100, ErrorMessage = "Years of experience must be between 1 and 100")]
    public short YearsOfExperience { get; set; }

    [StringLength(50, ErrorMessage = "Languages cannot exceed 50 characters")]
    public string? Languages { get; set; }

    [Required(ErrorMessage = "Certification information is required")]
    [StringLength(500, ErrorMessage = "Certification details cannot exceed 500 characters")]
    public string Certification { get; set; } = null!;

	public string? FacebookAccount { get; set; }

	public string? LinkedInAccount { get; set; }

	public string? InstagramAccount { get; set; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public List<IFormFile> Photo { get; set; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public List<string> PhotoType { get; set; }

	public List<PhotoModel> Photos { get; set; } = new List<PhotoModel>();
}

public class PhotoModel
{
	public int Id { get; set; }
	public string PhotoUrl { get; set; } = string.Empty;
	public string? Type { get; set; }
}
