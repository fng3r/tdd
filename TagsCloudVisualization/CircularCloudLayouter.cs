using System;
using System.Collections.Generic;
using System.Drawing;
using FluentAssertions;
using NUnit.Framework;

namespace TagsCloudVisualization
{
    class CircularCloudLayouter
    {
        public List<Rectangle> Rectangles { get; }

        private readonly Point center;

        public CircularCloudLayouter(Point center)
        {
            this.center = center;
            Rectangles = new List<Rectangle>();
        }

        public Rectangle PutNextRectangle(Size rectSize)
        {
            var rectangle = new Rectangle(center, rectSize);
            Rectangles.Add(rectangle);

            return rectangle;
        }
    }

    [TestFixture]
    public class CircularCloudLayouter_Should
    {
        private Point center;
        private CircularCloudLayouter layouter;

        [SetUp]
        public void SetUp()
        {
            center = new Point(0, 0);
            layouter = new CircularCloudLayouter(center);
        }


        [Test]
        public void Place_FirstRectangle_AtCenter()
        {
            layouter.PutNextRectangle(new Size(10, 10)).Location.Should().Be(center);
        }

        [TestCase(0, 0)]
        [TestCase(10, 200)]
        [TestCase(1000, 50)]
        public void PutRectangle_WithGivenSize(int width, int height)
        {
            var rectSize = new Size(width, height);
            layouter.PutNextRectangle(rectSize).Size.Should().Be(rectSize);
        }

        [TestCase(0)]
        [TestCase(10)]
        [TestCase(10000)]
        public void Save_AllPlacedRectangles(int count)
        {
            for (int i = 0; i < count; i++)
                layouter.PutNextRectangle(new Size(10, 10));

            layouter.Rectangles.Should().HaveCount(count);
        }
    }
}
