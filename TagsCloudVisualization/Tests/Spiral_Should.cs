using System.Drawing;
using System.Windows.Forms;
using FluentAssertions;
using NUnit.Framework;
using TagsCloudVisualization.Spiral;

namespace TagsCloudVisualization.Tests
{
	[TestFixture]
	public class Spiral_Should
	{
		[Test]
		[Explicit]
		public void DrawSpiral()
		{
			var vizualizer = new SpiralVizualizer(new Point(250, 250))
			{
				Size = new Size(500, 500)
			};

			Application.Run(vizualizer);
		}

		[Test]
		public void IncrementAngleAndRadiusByDelta_OnNextStep()
		{
			const int deltaAngle = 1;
			const int deltaRadius = 2;
			var spiral = new Spiral.Spiral(new Point(100, 100), deltaAngle, deltaRadius);
			var previousAngle = spiral.CurrentAngle;
			var previousRadius = spiral.CurrentRadius;
			spiral.GetNextSpiralPoint();
			(spiral.CurrentAngle - previousAngle).Should().Be(deltaAngle);
			(spiral.CurrentRadius - previousRadius).Should().Be(deltaRadius);
		}
	}
}