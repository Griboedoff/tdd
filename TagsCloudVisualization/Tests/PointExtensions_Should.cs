using System.Drawing;
using NUnit.Framework;

namespace TagsCloudVisualization.Tests
{
	[TestFixture]
	public class PointExtensions_Should
	{
		[TestCase(1, 0, ExpectedResult = 1)]
		[TestCase(-1312, 0, ExpectedResult = -1)]
		[TestCase(0, 0, ExpectedResult = 0)]
		public int SnapByX_Should_SaveSign(int x, int y)
		{
			return new Point(x, y).SnapByX().X;
		}

		[TestCase(0, 1, ExpectedResult = 1)]
		[TestCase(0, -123, ExpectedResult = -1)]
		[TestCase(0, 0, ExpectedResult = 0)]
		public int SnapByY_Should_SaveSign(int x, int y)
		{
			return new Point(x, y).SnapByY().Y;
		}
	}
}