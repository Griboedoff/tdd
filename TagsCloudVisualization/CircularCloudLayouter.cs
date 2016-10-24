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

		private bool IsInValidPosition(Rectangle checkingRect)
			=> !(PlacedRectangles.Any(rect => rect.IsIntersectedWith(checkingRect) || checkingRect.IsInside(rect))
			     || !checkingRect.IsInside(cloudBorders));

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
				throw new ArgumentException($"Size must be positive {rectangleSize}");

			var nextSpiralPoint = spiral.GetNextSpiralPoint();
			var location = new Point(nextSpiralPoint.X - rectangleSize.Width / 2, nextSpiralPoint.Y - rectangleSize.Height / 2);

			var nextRectangle = new Rectangle(
				location,
				rectangleSize);

			while (!IsInValidPosition(nextRectangle))
			{
				nextSpiralPoint = spiral.GetNextSpiralPoint();
				nextRectangle =
					new Rectangle(
						new Point(nextSpiralPoint.X - rectangleSize.Width / 2, nextSpiralPoint.Y - rectangleSize.Height / 2),
						rectangleSize);
				if (!cloudBorders.Contains(nextSpiralPoint))
					return Rectangle.Empty;
			}
			MoveToCenter(ref nextRectangle);
			PlacedRectangles.Add(nextRectangle);
			return nextRectangle;
		}

		private void MoveToCenter(ref Rectangle rect)
		{
			var horizontalyMoved = true;
			var verticalyMoved = true;
			while (horizontalyMoved || verticalyMoved)
			{
				var vectToCenter = center.Sub(new Point(rect.Location.X + rect.Width / 2, rect.Location.Y + rect.Height / 2));
				horizontalyMoved = TryMove(ref rect, vectToCenter.SnapByX());
				verticalyMoved = TryMove(ref rect, vectToCenter.SnapByY());
			}
		}

		private bool TryMove(ref Rectangle rect, Point shift)
		{
			if (shift == new Point(0, 0))
				return false;
			var newRect = new Rectangle(rect.Location.Add(shift), rect.Size);
			var isInValidPosition = IsInValidPosition(newRect);
			if (isInValidPosition)
				rect = newRect;
			return isInValidPosition;
		}

		public Bitmap ToBitmap()
		{
			var image = new Bitmap(cloudBorders.Width, cloudBorders.Height);
			var g = Graphics.FromImage(image);
			g.FillRectangle(Brushes.Black, cloudBorders);

			foreach (var rectangle in PlacedRectangles)
				g.FillRectangle(Brushes.Aquamarine, rectangle);
			g.DrawRectangle(Pens.Red, new Rectangle(center, new Size(1, 1)));

			return image;
		}
	}
}