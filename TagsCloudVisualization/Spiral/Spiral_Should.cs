using System.Drawing;
using System.Windows.Forms;
using NUnit.Framework;

namespace TagsCloudVisualization.Spiral
{
	[TestFixture]
	public class Spiral_Should
	{
		[Test]
		[Explicit]
		public void DrawSpiral()
		{
			var vizualizer = new SpiralVizualizer(new Point(250, 250))
			{
				Size = new Size(500, 500)
			};

			Application.Run(vizualizer);
		}
	}
}