using System.IO;
using TheForestWaiter.Shared;

namespace TheForestWaiter.Content
{
	public static class ContentTypeDefinitions
	{
		public static ContentType FromFilename(string filename)
		{
			var ext = Path.GetExtension(filename);
			return ext switch
			{
				".png" or ".jpg" => ContentType.Texture,
				".ttf" => ContentType.Font,
				".particle" => ContentType.Particle,
				".wav" => ContentType.Sound,
				_ => ContentType.Raw,
			};
		}
	}
}
