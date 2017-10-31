using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
            if (Rectangles.Count > 0)
            {
                var lastRectLocation = Rectangles[Rectangles.Count - 1].Location;
                var rectangleLocation = lastRectLocation - (rectSize + new Size(1, 1));
                rectangle.Location = rectangleLocation;
            }
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

        [Test]
        public void NotContain_IntersectedRectangles()
        {
            PutRandomRectangles(100);
            var rectangles = layouter.Rectangles;
            rectangles.All(rect => rectangles.All(otherRect => otherRect == rect || !rect.IntersectsWith(otherRect))).Should().BeTrue();
        }

        [Test]
        public void EvenlyDistribute_Rectangles()
        {
            var maxRectBound = 100;
            PutRandomRectangles(100, maxRectBound);

            var rectangles = layouter.Rectangles;
            var dx1 = rectangles.Min(r => Math.Abs(r.Center().X - center.X));
            var dx2 = rectangles.Max(r => Math.Abs(r.Center().X - center.X));
            var dy1 = rectangles.Max(r => Math.Abs(r.Center().Y - center.Y));
            var dy2 = rectangles.Min(r => Math.Abs(r.Center().Y - center.Y));
            var deltas = new[] { dx1, dx2, dy1, dy2 };
            Console.WriteLine($"{dx1}, {dx2}, {dy1}, {dy2}");

            deltas
                .SelectMany(c1 => deltas.Select(c2 => (c1, c2)))
                .Max(t => Math.Abs(t.Item1 - t.Item2))
                .Should().BeInRange(0, maxRectBound);
        }

        private void PutRandomRectangles(int count, int maxBound = 100)
        {
            foreach (var size in GetRandomRectangleSizes(count, maxBound))
                layouter.PutNextRectangle(size);
        }

        private IEnumerable<Size> GetRandomRectangleSizes(int count, int maxBound)
        {
            Random rnd = new Random();
            return GenerateSizes().Take(count);

            IEnumerable<Size> GenerateSizes()
            {
                while (true)
                    yield return new Size(rnd.Next(maxBound), rnd.Next(maxBound));
                // ReSharper disable once IteratorNeverReturns
            }
        }
    }
}
