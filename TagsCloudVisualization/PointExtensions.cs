using System;
using System.Drawing;

namespace TagsCloudVisualization
{
	public static class PointExtensions
	{
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