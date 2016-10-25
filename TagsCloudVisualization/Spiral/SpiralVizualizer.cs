using System.Drawing;
using System.Windows.Forms;

namespace TagsCloudVisualization.Spiral
{
	public class SpiralVizualizer : Form
	{
		private readonly Point center;

		public SpiralVizualizer(Point center)
		{
			this.center = center;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var graphics = e.Graphics;
			var spiral = new Spiral(center);
			var prev = spiral.GetNextSpiralPoint();
			for (var i = 0; i < 1000; i++)
			{
				var item = spiral.GetNextSpiralPoint();
				graphics.DrawLine(Pens.Orange, prev, item);
				prev = item;
			}
		}
	}
}