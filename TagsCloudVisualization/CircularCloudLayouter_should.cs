using System;
using System.Drawing;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace TagsCloudVisualization
{
	[TestFixture]
	internal class CircularCloudLayouter_Should
	{
		private CircularCloudLayouter cloud { get; set; }

		[SetUp]
		public void SetUp()
		{
			cloud = new CircularCloudLayouter(new Point(100, 100));
		}

		[Test]
		public void BeEmpty_AfterCreation()
		{
			cloud.PlacedRectangles.Should().BeEmpty();
		}

		[TestCase(-1, 0)]
		[TestCase(0, -1)]
		[TestCase(-1, -1)]
		public void ThrowArgumentException_IfCenterPointIsNegative(int x, int y)
		{
			Assert.Throws<ArgumentException>(() => new CircularCloudLayouter(new Point(x, y)));
		}


		[TestCase(10, 10)]
		public void PlaceRectInCenter_IfItIsOne(int x, int y)
		{
			cloud.PutNextRectangle(new Size(x, y));
			cloud.PlacedRectangles.First().Location.Should().Be(cloud.Center);
		}
	}
}