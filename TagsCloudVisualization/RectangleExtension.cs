using System.Drawing;
using NUnit.Framework.Constraints;

namespace TagsCloudVisualization
{
	public static class RectangleExtension
	{
		public static bool IsIntersectedWith(this Rectangle r1, Rectangle r2)
		{
			return r1.Bottom >= r2.Top && r1.Right >= r2.Left && r2.Bottom >= r1.Top && r2.Right >= r1.Left;
		}

		public static bool IsInside(this Rectangle r1, Rectangle r2)
		{
			return r1.IsIntersectedWith(r2) &&
			       r1.Right <= r2.Right && r1.Left >= r2.Left && r1.Bottom <= r2.Bottom && r1.Top >= r2.Top;
		}

		public static Point GetCenter(this Rectangle rect)
		{
			return rect.Location.Add(new Point(rect.Width / 2, rect.Height / 2));
		}
	}
}