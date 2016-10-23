using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TagsCloudVisualization
{
	internal class CircularCloudLayouter
	{
		private Point center;
		private readonly Spiral spiral;
		private readonly Rectangle cloudBorders;

		public Point Center
		{
			get { return center; }
			private set
			{
				if (value.X < 0 || value.Y < 0)
					throw new ArgumentException("Center point should be non-negative");
				center = value;
			}
		}

		public List<Rectangle> PlacedRectangles { get; }

		public CircularCloudLayouter(Point center)
		{
			spiral = new Spiral(center);
			Center = center;
			cloudBorders = new Rectangle(0, 0, center.X * 2, center.Y * 2);
			PlacedRectangles = new List<Rectangle>();
		}

		public Rectangle PutNextRectangle(Size rectangleSize)
		{
			if (rectangleSize.Height <= 0 || rectangleSize.Width <= 0)
				throw new ArgumentException($"Size must be positive {rectangleSize.ToString()}");

			var nextSpiralPoint = spiral.GetNextSpiralPoint();
			var nextRectangle = new Rectangle(nextSpiralPoint, rectangleSize);

			while (PlacedRectangles
				.Any(rect => (rect.IsIntersectedWith(nextRectangle) || nextRectangle.IsInside(rect)))
				             || !nextRectangle.IsInside(cloudBorders))
			{
				nextSpiralPoint = spiral.GetNextSpiralPoint();
				nextRectangle = new Rectangle(nextSpiralPoint, rectangleSize);
				if (!cloudBorders.Contains(nextSpiralPoint))
					return Rectangle.Empty;
			}

			PlacedRectangles.Add(nextRectangle);
			return nextRectangle;
		}

		public Bitmap ToBitmap()
		{
			var image = new Bitmap(cloudBorders.Width, cloudBorders.Height);
			var g = Graphics.FromImage(image);

			g.FillRectangle(Brushes.Black, cloudBorders);

			foreach (var rectangle in PlacedRectangles)
				g.DrawRectangle(Pens.Aquamarine, rectangle);

			return image;
		}
	}
}