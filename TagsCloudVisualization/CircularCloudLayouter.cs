using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TagsCloudVisualization
{
	internal class CircularCloudLayouter
	{
		private Point center;
		private readonly Spiral.Spiral spiral;
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

		public int Count => PlacedRectangles.Count;

		private List<Rectangle> PlacedRectangles { get; }

		private bool IsInValidPosition(Rectangle checkingRectangle)
			=> !PlacedRectangles.Any(rect => rect.IntersectsWith(checkingRectangle)) && cloudBorders.Contains(checkingRectangle);

		private static Point GetRectangleCenterLocation(Size rectangleSize, Point nextSpiralPoint)
			=> new Point(nextSpiralPoint.X - rectangleSize.Width / 2, nextSpiralPoint.Y - rectangleSize.Height / 2);

		public CircularCloudLayouter(Point center)
		{
			spiral = new Spiral.Spiral(center);
			Center = center;
			cloudBorders = new Rectangle(0, 0, center.X * 2, center.Y * 2);
			PlacedRectangles = new List<Rectangle>();
		}

		public Rectangle PutNextRectangle(Size rectangleSize)
		{
			if (rectangleSize.Height <= 0 || rectangleSize.Width <= 0)
				throw new ArgumentException($"Size must be positive {rectangleSize}");

			var nextRectangle = FindNextRectanglePosition(rectangleSize);

			if (nextRectangle.IsEmpty) return nextRectangle;

			nextRectangle = MoveToCenter(nextRectangle);
			PlacedRectangles.Add(nextRectangle);
			return nextRectangle;
		}

		private Rectangle FindNextRectanglePosition(Size rectangleSize)
		{
			var nextSpiralPoint = spiral.GetNextSpiralPoint();
			var nextRectangle = new Rectangle(GetRectangleCenterLocation(rectangleSize, nextSpiralPoint), rectangleSize);

			while (!IsInValidPosition(nextRectangle))
			{
				nextSpiralPoint = spiral.GetNextSpiralPoint();
				nextRectangle = new Rectangle(GetRectangleCenterLocation(rectangleSize, nextSpiralPoint), rectangleSize);
				if (!cloudBorders.Contains(nextSpiralPoint))
					return Rectangle.Empty;
			}

			return nextRectangle;
		}

		private Rectangle MoveToCenter(Rectangle rectangle)
		{
			var newRectangle = Rectangle.Empty;
			while (rectangle != newRectangle)
			{
				if(!newRectangle.IsEmpty)
					rectangle = newRectangle;
				var vectorToCenter = center - new Size(rectangle.GetCenter());
				newRectangle = TryMove(rectangle, vectorToCenter.SnapByX());
				newRectangle = TryMove(newRectangle, vectorToCenter.SnapByY());
			}
			return rectangle;
		}

		private Rectangle TryMove(Rectangle rectangle, Point shift)
		{
			var newRect = new Rectangle(rectangle.Location + new Size(shift), rectangle.Size);
			var isInValidPosition = IsInValidPosition(newRect);
			if (isInValidPosition)
				rectangle = newRect;
			return rectangle;
		}

		public Bitmap ToBitmap()
		{
			var image = new Bitmap(cloudBorders.Width, cloudBorders.Height);
			var g = Graphics.FromImage(image);
			g.FillRectangle(Brushes.Black, cloudBorders);

			foreach (var rectangle in PlacedRectangles)
			{
				g.FillRectangle(Brushes.Aquamarine, rectangle);
				g.DrawRectangle(Pens.Blue, rectangle);
			}
			g.DrawRectangle(Pens.Red, new Rectangle(center, new Size(1, 1)));

			return image;
		}
	}
}