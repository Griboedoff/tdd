using System;
using System.Drawing;

namespace TagsCloudVisualization.Spiral
{
	public class Spiral
	{
		private readonly Point center;
		private readonly double deltaAngle;
		private readonly double deltaRadius;
		private Point currentPoint;
		private double currentAngle;
		private double currentRadius;

		public Spiral(Point center, double deltaAngle = Math.PI / 180, double deltaRadius = 0.001)
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
}