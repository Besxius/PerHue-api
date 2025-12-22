using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PerHue.Application.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class AllowedImageExtensionsAttribute : ValidationAttribute
	{
		private readonly string[] _extensions;

		public AllowedImageExtensionsAttribute(params string[] extensions)
		{
			_extensions = extensions;
		}

		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		{
			if (value == null)
			{
				return ValidationResult.Success;
			}

			if (value is IFormFile file)
			{
				return ValidateFile(file);
			}

			if (value is IEnumerable<IFormFile> files)
			{
				foreach (var f in files)
				{
					var result = ValidateFile(f);
					if (result != ValidationResult.Success)
					{
						return result;
					}
				}
			}

			return ValidationResult.Success;
		}

		private ValidationResult? ValidateFile(IFormFile file)
		{
			var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

			if (string.IsNullOrEmpty(extension) || !_extensions.Contains(extension))
			{
				var allowedExtensions = string.Join(", ", _extensions);
				return new ValidationResult(
					ErrorMessage ?? $"Only the following image formats are allowed: {allowedExtensions}. Received: {extension}"
				);
			}

			return ValidationResult.Success;
		}
	}
}