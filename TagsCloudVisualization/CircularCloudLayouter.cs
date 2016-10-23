using System;
using System.Collections.Generic;
using System.Drawing;
using NUnit.Framework;

namespace TagsCloudVisualization
{
	internal class CircularCloudLayouter
	{
		private Point center;

		public Point Center
		{
			get { return center; }
			set
			{
				if (value.X < 0 || value.Y < 0)
					throw new ArgumentException("Center point should be non-negative");
				center = value;
			}
		}

		public List<Rectangle> PlacedRectangles { get; }

		public CircularCloudLayouter(Point center)
		{
			Center = center;
			PlacedRectangles = new List<Rectangle>();
		}

		public Rectangle PutNextRectangle(Size rectangleSize)
		{
			var nextRectangle = new Rectangle(Center, rectangleSize);
			PlacedRectangles.Add(nextRectangle);
			return nextRectangle;
		}
	}
}