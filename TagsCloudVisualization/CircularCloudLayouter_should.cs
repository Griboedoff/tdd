using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace TagsCloudVisualization
{
	[TestFixture]
	internal class CircularCloudLayouter_Should
	{
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
				var size = rand.Next(1, maxSize + 1);
				generatedSizes.Add(new Size(size, size));
			}
			return generatedSizes.OrderByDescending(x => x.Width);
		}

		private static IEnumerable<Size> GenerateRectangles(int maxSize, int numberOfRectangles, int seed)
		{
			var generatedSizes = new List<Size>();
			var rand = new Random(seed);
			for (var i = 0; i < numberOfRectangles; i++)
				generatedSizes.Add(new Size(rand.Next(1, maxSize + 1), rand.Next(1, maxSize + 1)));
			return generatedSizes.OrderByDescending(x => x.Width + x.Height);
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
			Cloud.PlacedRectangles.First().Location.Should().Be(Cloud.Center);
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
			var squares = GenerateSquares(20, 2000, 10);
			foreach (var square in squares)
				cloud.PutNextRectangle(square);
			var filename = Path.Combine(
				TestContext.CurrentContext.TestDirectory,
				"examples",
				(int) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds + ".bmp");
			var bitmap = cloud.ToBitmap();
			bitmap.Save(filename);
		}

//		[TearDown]
//		public void TearDown()
//		{
//			if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
//			{
//				var filename = Path.Combine(TestContext.CurrentContext.TestDirectory,
//					"TestFailures",
//					TestContext.CurrentContext.Test.MethodName,
//					TestContext.CurrentContext.Test.ID + ".bmp");
//				Cloud.ToBitmap().Save(filename);
//				Console.WriteLine($"bitmap saved to {filename}");
//			}
//		}
	}
}