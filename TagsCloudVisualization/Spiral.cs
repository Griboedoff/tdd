using System;
using System.Collections.Generic;
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

		public Spiral(Point center, double deltaAngle = 10, double deltaRadius = 1)
		{
			this.center = center;
			this.deltaAngle = deltaAngle;
			this.deltaRadius = deltaRadius;
			currentPoint = this.center;
			currentAngle = 0;
			currentRadius = 0;
		}

		public IEnumerable<Point> GetNextSpiralPoint()
		{
			yield return currentPoint;
			currentAngle += deltaAngle;
			currentRadius += deltaRadius;

			var newX = (int) (currentRadius * Math.Cos(currentAngle) + center.X);
			var newY = (int) (currentRadius * Math.Sin(currentAngle) + center.Y);
			currentPoint = new Point(newX, newY);
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
			var i = 100;
			foreach (var item in spiral.GetNextSpiralPoint())
			{
				if (i-- == 0)
					break;
				graphics.DrawRectangle(Pens.Black, item.X, item.Y, 1, 1);
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