using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace TagsCloudVisualization
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


		[Test]
		public void BeEmpty_AfterCreation()
		{
			Cloud.PlacedRectangles.Should().BeEmpty();
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

		[TestCase(10, 10)]
		[TestCase(13, 53)]
		public void PlaceRectInCenter_IfItIsOne(int x, int y)
		{
			var rectangleSize = new Size(x, y);
			Cloud.PutNextRectangle(rectangleSize);
			var rect = Cloud.PlacedRectangles.First();
			rect.GetCenter().Should().Be(Cloud.Center);
			Cloud.PlacedRectangles.First().Size.Should().Be(rectangleSize);
		}

		[Test]
		public void PlaceTwoRectangles_WithoutIntersection()
		{
			var rectangleSize = new Size(10, 12);
			var rectangleSize2 = new Size(10, 10);
			Cloud.PutNextRectangle(rectangleSize);
			Cloud.PutNextRectangle(rectangleSize2);
			Cloud.PlacedRectangles.Should().HaveCount(2);
			Cloud.PlacedRectangles[0].IsIntersectedWith(Cloud.PlacedRectangles[1]).Should().BeFalse();
		}

		[Test]
		public void NotPlace_IfRectIsOutsideCloudBorder()
		{
			var size = new Size(1000, 10);
			Cloud.PutNextRectangle(size);
			Cloud.PlacedRectangles.Should().BeEmpty();
		}

		[TestCase(10, 100)]
		[TestCase(100, 20)]
		[TestCase(1000, 2)]
		public void AddManyRectangles(int number, int maxSize)
		{
			var squares = GenerateSquares(maxSize, number, 10);
			foreach (var square in squares)
				Cloud.PutNextRectangle(square);
			Cloud.PlacedRectangles.Should().HaveCount(number);
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
			var filename = Path.Combine(
				TestContext.CurrentContext.TestDirectory,
				"examples",
				GetCurrentTimeStamp() + ".png");
			var bitmap = cloud.ToBitmap();
			bitmap.Save(filename, ImageFormat.Png);
		}

		[TearDown]
		public void TearDown()
		{
			if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
			{
				var filename = Path.Combine(TestContext.CurrentContext.TestDirectory,
					string.Join(".", GetCurrentTimeStamp(), TestContext.CurrentContext.Test.MethodName, "png"));
				Cloud.ToBitmap().Save(filename, ImageFormat.Png);
				Console.WriteLine($"bitmap saved to {filename}");
			}
		}
	}
}