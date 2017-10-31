using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace TagsCloudVisualization
{
    public class Spiral : IEnumerable<Point>
    {
        private readonly PointF initialPoint;

        public Spiral(PointF initialPoint) => this.initialPoint = initialPoint;

        public IEnumerator<Point> GetEnumerator()
        {
            var t = 0.01;
            while (true)
            {
                var x = (int)(initialPoint.X + Math.Exp(0.001 * t) * Math.Cos(t));
                var y = (int)(initialPoint.Y + Math.Exp(0.001 * t) * Math.Sin(t));
                yield return new Point(x, y);
                t += 0.05;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}