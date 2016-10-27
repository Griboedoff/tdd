using System.Drawing;
using System.Windows.Forms;

namespace TagsCloudVisualization.Tests
{
	internal class ImageForm : Form
	{
		private readonly Image image;

		public ImageForm(Image image)
		{
			this.image = image;
			Size = image.Size;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var g = e.Graphics;
			g.DrawImage(image, new Point(0, 0));
		}
	}
}