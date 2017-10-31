using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NUnit.Framework;

namespace TagsCloudVisualization
{
    public static class RectangleExtensions
    {
        public static Point Center(this Rectangle rect) => new Point();
    }

    [TestFixture]
    public class RectangleExtensionsTests
    {
        [TestCaseSource(typeof(RectangleTestData), nameof(RectangleTestData.CenterTestData))]
        public Point Center(Rectangle rect)
        {
            return rect.Center();
        }
    }

    public class RectangleTestData
    {
        public static IEnumerable CenterTestData
        {
            get
            {
                yield return new TestCaseData(new Rectangle(0, 0, 10, 20)).Returns(new Point(5, 10));
                yield return new TestCaseData(new Rectangle(0, 100, 50, 50)).Returns(new Point(25, 125));
                yield return new TestCaseData(new Rectangle(-100, -50, 200, 50)).Returns(new Point(0, -25));
            }
        }
    }
}
