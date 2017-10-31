using System.Collections.Generic;
using System.Drawing;

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
}
