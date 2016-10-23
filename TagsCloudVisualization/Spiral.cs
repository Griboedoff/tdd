using System;
using System.Drawing;
using System.Windows.Forms;
using NUnit.Framework;

namespace TagsCloudVisualization
{
	public class Spiral
	{
		private readonly Point center;
		private readonly double deltaAngle;
		private readonly double deltaRadius;
		private Point currentPoint;
		private double currentAngle;
		private double currentRadius;

		public Spiral(Point center, double deltaAngle = Math.PI / 90, double deltaRadius = 0.001)
		{
			this.center = center;
			this.deltaAngle = deltaAngle;
			this.deltaRadius = deltaRadius;
			currentPoint = this.center;
			currentAngle = 0;
			currentRadius = 0;
		}

		public Point GetNextSpiralPoint()
		{
			var prevPoint = currentPoint;
			currentAngle += deltaAngle;
			currentRadius += deltaRadius;

			var newX = (int) (currentRadius * Math.Cos(currentAngle) + center.X);
			var newY = (int) (currentRadius * Math.Sin(currentAngle) + center.Y);
			currentPoint = new Point(newX, newY);
			return prevPoint;
		}
	}

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