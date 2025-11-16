using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Application.Models.ExpertTestResult
{
	public class CreateExpertTestRequestModel
	{
		[Required(ErrorMessage = "Please upload an image.")]
		public IFormFile File { get; set; }

		public string? HairColor { get; set; }

		public string? EyesColor { get; set; }

		public string? LipsColor { get; set; }

		public string? SkinColor { get; set; }
	}
}
