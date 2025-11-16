using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PerHue.Application.Models
{
	public class TestRequestModel
	{
		public int Id { get; set; }
		public string? HairColor { get; set; }
		public string? EyesColor { get; set; }
		public string? LipsColor { get; set; }
		public string? SkinColor { get; set; }
		public string? Status { get; set; }
		public DateTime? CreatedDate { get; set; }
		public string TypeOfTest { get; set; } = null!;
		public int UserAccountId { get; set; }
		public virtual ICollection<AiPictureModel> AiPictures { get; set; } = new List<AiPictureModel>();
	}

	// You can create a simple model for the pictures
	public class AiPictureModel
	{
		public int Id { get; set; }
		public string Source { get; set; } = null!;
		public string Note { get; set; } = null!;
	}
}