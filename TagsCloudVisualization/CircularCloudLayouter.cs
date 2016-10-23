using System;
using System.Collections.Generic;
using System.Drawing;
using NUnit.Framework;

namespace TagsCloudVisualization
{
	internal class CircularCloudLayouter
	{
		public Point Center
		{
			get { return Center; }
			set
			{
				if (value.X < 0 || value.Y < 0)
					throw new ArgumentException("Center point should be non-negative");
				Center = value;
			}
		}

		public List<Rectangle> PlacedRectangles { get; private set; }

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