using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace TagsCloudVisualization
{
	public static class BitmapExtension
	{
		public static void Save(this Bitmap b, string path)
		{
			using (var memory = new MemoryStream())
			{
				using (var fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
				{
					b.Save(memory, ImageFormat.Bmp);
					var bytes = memory.ToArray();
					fs.Write(bytes, 0, bytes.Length);
				}
			}
		}
	}
}