using System.Drawing;

namespace TagsCloudVisualization
{
	public static class RectangleExtension
	{
		public static Point GetCenter(this Rectangle rect)
		{
			return rect.Location.Add(new Point(rect.Width / 2, rect.Height / 2));
		}
	}
}