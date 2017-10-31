using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TagsCloudVisualization
{
    public static class Program
    {
        private static readonly int Width = Screen.PrimaryScreen.Bounds.Width;
        private static readonly int Height = Screen.PrimaryScreen.Bounds.Height;

        public static void Main()
        {
            var visualizer = new TagsCloudVisualizer(
                config => config
                    .SetBackground(Color.LightSteelBlue)
                    .SetForeground(Color.OrangeRed)
            );

            DrawRectangles(100, visualizer);
            DrawWords(150, visualizer);
        }

        private static void DrawRectangles(int count, TagsCloudVisualizer visualizer)
        {
            var sizeGenerators = new[] { GenerateRectangleSizes1(), GenerateRectangleSizes2() };

            for (int i = 0; i < sizeGenerators.Length; i++)
            {
                var center = new Point(Width / 2, Height / 2);
                var layouter = new CircularCloudLayouter(center);
                foreach (var rectSize in sizeGenerators[i].Take(count))
                    layouter.PutNextRectangle(rectSize);

                var bitmap = visualizer.DrawRectangles(Width, Height, layouter.Rectangles);
                bitmap.Save($@"..\..\Examples\RectangleCloud{i}.png");
            }
        }

        public static void DrawWords(int count, TagsCloudVisualizer visualizer)
        {
            IEnumerable<Word> words = GetWords().Take(count);

            var center = new Point(Width / 2, Height / 2);
            var layouter = new CircularCloudLayouter(center);
            words = words.Select(word =>
            {
                word.Area = layouter.PutNextRectangle(word.Size);
                return word;
            });

            var bitmap = visualizer.DrawWords(Width, Height, words);
            bitmap.Save(@"..\..\Examples\WarAndPeaceCloud.png");
        }

        public static IEnumerable<Word> GetWords()
        {
            var lines = File.ReadLines(@"data\war_and_peace.txt");
            var wordDelimiters = new[] { '.', ',', ';', ' ', ':', '(', ')', '[', ']', '\'', '"', '?', '!', '–', '\n' };
            var mostFrequentWords = lines
                .SelectMany(line => line.Split(wordDelimiters, StringSplitOptions.RemoveEmptyEntries))
                .Where(w => w.Length > 3)
                .Select(w => w.ToLower())
                .GroupBy(w => w)
                .Select(group => (word: group.Key, count: group.Count()))
                .OrderByDescending(t => t.count);

            var measurer = new TextMeasurer();
            var largestCount = mostFrequentWords.First().count;
            var words = mostFrequentWords
                .Select(e => (word: e.word, weight: (double)e.count / largestCount))
                .Select(e => measurer.MeasureText(e.word, e.weight));
            return words;
        }

        private static IEnumerable<Size> GenerateRectangleSizes1()
        {
            var rnd = new Random();
            while (true)
                yield return new Size((int)(1.1 * rnd.Next(100)), 20 + (int)(0.7 * rnd.Next(100)));
            // ReSharper disable once IteratorNeverReturns
        }

        private static IEnumerable<Size> GenerateRectangleSizes2()
        {
            var rnd = new Random(42);
            while (true)
            {
                var rndInt = rnd.Next(60);
                yield return new Size(Math.Max(60, 3 * rndInt), Math.Max(20, rndInt));
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}
