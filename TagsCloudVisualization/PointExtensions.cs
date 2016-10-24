using System;
using System.Drawing;

namespace TagsCloudVisualization
{
	public static class PointExtensions
	{
		public static Point Add(this Point p1, Point p2)
		{
			return new Point(p1.X + p2.X, p1.Y + p2.Y);
		}

		public static Point Sub(this Point p1, Point p2)
		{
			return new Point(p1.X - p2.X, p1.Y - p2.Y);
		}

		public static Point SnapByX(this Point p)
		{
			return new Point(p.X / (p.X != 0 ? Math.Abs(p.X) : 1), 0);
		}

		public static Point SnapByY(this Point p)
		{
			return new Point(0, p.Y / (p.Y != 0 ? Math.Abs(p.Y) : 1));
		}
	}
}