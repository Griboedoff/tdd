using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace TagsCloudVisualization.Tests
{
	[TestFixture]
	internal class CircularCloudLayouter_Should
	{
		private static int GetCurrentTimeStamp() => (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
		private CircularCloudLayouter Cloud { get; set; }

		[SetUp]
		public void SetUp()
		{
			Cloud = new CircularCloudLayouter(new Point(300, 300));
		}

		private static IEnumerable<Size> GenerateSquares(int maxSize, int numberOfSquares, int seed)
		{
			var generatedSizes = new List<Size>();
			var rand = new Random(seed);
			for (var i = 0; i < numberOfSquares; i++)
			{
				var size = rand.Next(10, maxSize + 1);
				generatedSizes.Add(new Size(size, size));
			}
			return generatedSizes;
		}

		private static IEnumerable<Size> GenerateRectangles(int maxWidth, int maxHeigth, int numberOfRectangles, int seed)
		{
			var generatedSizes = new List<Size>();
			var rand = new Random(seed);
			for (var i = 0; i < numberOfRectangles; i++)
			{
				var width = rand.Next(1, maxWidth + 1);
				var height = rand.Next(10, maxHeigth + 1);
				generatedSizes.Add(new Size(width, height));
			}
			return generatedSizes;
		}

		private static IEnumerable<Size> GenerateWordLikeRectangles(int maxWidth, int maxHeigth, int numberOfRectangles,
			int seed)
		{
			GenerateRectangles(maxWidth, maxHeigth, numberOfRectangles, seed);
			var result = new List<Size>();
			while (result.Count < numberOfRectangles)
			{
				result = result
					.Concat(GenerateRectangles(maxWidth, maxHeigth, numberOfRectangles, seed).Where(size => size.Width > size.Height))
					.ToList();
			}
			return result.Take(numberOfRectangles);
		}

		private static string GetFileName(IEnumerable<string> additionalPathParts)
		{
			var pathParts =
				new[] {TestContext.CurrentContext.TestDirectory}
					.Concat(additionalPathParts
						.Concat(new[] {string.Join(".", GetCurrentTimeStamp(), ".png")}))
					.ToArray();
			return Path.Combine(pathParts);
		}

		[Test]
		public void BeEmpty_AfterCreation()
		{
			Cloud.Count.Should().Be(0);
		}

		[TestCase(-1, 0)]
		[TestCase(0, -1)]
		[TestCase(-1, -1)]
		public void ThrowArgumentException_IfCenterPointIsNegative(int x, int y)
		{
			// ReSharper disable once ObjectCreationAsStatement
			Assert.Throws<ArgumentException>(() => new CircularCloudLayouter(new Point(x, y)));
		}

		[Test]
		public void ThrowArgumentException_IfSizeIsNegative()
		{
			Assert.Throws<ArgumentException>(() => Cloud.PutNextRectangle(new Size(-10, -10)));
		}

		[Test]
		public void ThrowException_IfCloudIsTooSmall()
		{
			var size = new Size(1000, 10);

			Assert.Throws<ArgumentException>(() => Cloud.PutNextRectangle(size));
		}

		[Test]
		public void ThrowException_IfTooMuchRectangles()
		{
			var sizes = GenerateRectangles(100, 100, 5000, 10);

			Assert.Throws<ArgumentException>(() =>
			{
				foreach (var size in sizes)
				{
					Cloud.PutNextRectangle(size);
				}
			});
		}

		[TestCase(10, 10)]
		[TestCase(13, 53)]
		public void PlaceRectInCenter_IfItIsOne(int x, int y)
		{
			var rectangleSize = new Size(x, y);

			var rect = Cloud.PutNextRectangle(rectangleSize);

			rect.GetCenter().Should().Be(Cloud.Center);
			rect.Size.Should().Be(rectangleSize);
		}

		[Test]
		public void PlaceTwoRectangles_WithoutIntersection()
		{
			var rectangleSize = new Size(10, 12);
			var rectangleSize2 = new Size(10, 10);

			var rect1 = Cloud.PutNextRectangle(rectangleSize);
			var rect2 = Cloud.PutNextRectangle(rectangleSize2);

			Cloud.Count.Should().Be(2);
			rect1.IntersectsWith(rect2).Should().BeFalse();
		}

		[TestCase(10, 100)]
		[TestCase(100, 20)]
		[TestCase(1000, 10)]
		public void AddManyRectangles(int number, int maxSize)
		{
			var squares = GenerateSquares(maxSize, number, 10);

			foreach (var square in squares)
				Cloud.PutNextRectangle(square);


			Cloud.Count.Should().Be(number);
		}

		[TestCase(100, 20)]
		[TestCase(1000, 10)]
		public void IsCloseToCircleShape_WhenPlacedManyRect(int number, int maxSize)
		{
			var squares = GenerateSquares(maxSize, number, 10).ToList();

			var lastPlaced = Rectangle.Empty;
			foreach (var square in squares)
				lastPlaced = Cloud.PutNextRectangle(square);

			var totalRectangleSquare = squares.Select(rectangle => rectangle.Height * rectangle.Width).Sum();
			var lastPlacedCenter = lastPlaced.GetCenter();
			var radius = Math.Pow(lastPlacedCenter.X - Cloud.Center.X, 2) + Math.Pow(lastPlacedCenter.Y - Cloud.Center.Y, 2);
			var circleSquare = Math.PI * radius;

			circleSquare.Should().BeInRange(0.9 * totalRectangleSquare, 1.1 * totalRectangleSquare);
		}

		[Test]
		[Explicit]
		public void MakeCloud_AndSaveToExamplesFolder()
		{
			var cloud = new CircularCloudLayouter(new Point(500, 500));
			var rects = GenerateSquares(40, 1000, 10);
//			var rects = GenerateRectangles(100, 30, 500, 10);
//			var rects = GenerateWordLikeRectangles(100, 30, 500, 10);

			foreach (var square in rects)
				cloud.PutNextRectangle(square);

			var filename = GetFileName(new[] {"examples"});
			var bitmap = cloud.ToBitmap();
			bitmap.Save(filename, ImageFormat.Png);
		}

		[TearDown]
		public void TearDown()
		{
			if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
			{
				var filename = GetFileName(new string[0]);
				Cloud.ToBitmap().Save(filename, ImageFormat.Png);
				Console.WriteLine($"bitmap saved to {filename}");
			}
		}
	}
}