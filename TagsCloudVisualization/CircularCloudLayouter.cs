using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using MoreLinq;

namespace TagsCloudVisualization
{
    public class CircularCloudLayouter
    {
        private readonly Point center;
        private readonly Spiral spiral;
        public List<Rectangle> Rectangles { get; }

        public CircularCloudLayouter(Point center)
        {
            this.center = center;
            spiral = new Spiral(center);
            Rectangles = new List<Rectangle>();
        }

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            foreach (var point in spiral)
            {
                var rectangleLocation = new Point(point.X - rectangleSize.Width / 2, point.Y - rectangleSize.Height / 2);
                var rectangle = new Rectangle(rectangleLocation, rectangleSize);
                if (rectangle.IntersectsWith(Rectangles)) continue;
                Rectangles.Add(rectangle);

                return rectangle;
            }

            return Rectangle.Empty;
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
            PutRandomRectangles(count);
            layouter.Rectangles.Should().HaveCount(count);
        }

        [Test]
        public void NotContain_IntersectedRectangles()
        {
            PutRandomRectangles(100);
            var rectangles = layouter.Rectangles;
            rectangles.All(rect => rectangles.All(otherRect => otherRect == rect || !rect.IntersectsWith(otherRect))).Should().BeTrue();
        }

        [TestCase(100, 50)]
        [TestCase(50, 500)]
        public void EvenlyDistribute_Rectangles(int count, int maxRectBound)
        {
            PutRandomRectangles(count, maxRectBound);

            var rectangles = layouter.Rectangles;
            var leftmostRect = rectangles.MinBy(r => r.Center().X);
            var rightmostRect = rectangles.MaxBy(r => r.Center().X);
            var uppermostRect = rectangles.MaxBy(r => r.Center().Y);
            var nethermostRect = rectangles.MinBy(r => r.Center().Y);

            var outerRects = new[] { leftmostRect, rightmostRect, uppermostRect, nethermostRect };

            var distancesToCenter = outerRects
                .Select(r => r.Center())
                .Select(p => GetVectorLength(p, center));

            distancesToCenter
                .SelectMany(d1 => distancesToCenter.Select(d2 => Math.Abs(d1 - d2)))
                .Max()
                .Should().BeInRange(0, maxRectBound);

            double GetVectorLength(Point a, Point b)
            {
                return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
            }
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
