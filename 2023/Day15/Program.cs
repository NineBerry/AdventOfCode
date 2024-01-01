using System.Text.RegularExpressions;

namespace Day15
{
    internal class Program
    {
        // private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day15\Sample.txt";
        private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day15\Full.txt";

        static void Main()
        {
            string[] lines = GetLines(fileName);
            string[] steps = lines.Single()
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            // Part 1
            int solutionPart1 = steps.Sum(CalculateHash);
            Console.WriteLine("Part 1:" + solutionPart1);

            // Part 2
            Dictionary<int, List<Lens>> boxes = new();

            foreach (var step in steps)
            {
                ProcessInitializationStep(step, boxes);
            }

            var solutionPart2 = CalculateFocusingPower(boxes);
            Console.WriteLine("Part 2:" + solutionPart2);

            Console.ReadLine();
        }

        private static int CalculateHash(string value)
        {
            // We could do some trickery with "unchecked" but why?

            int result = 0;
            foreach (char c in value)
            {
                result += c;
                result *= 17;
                result %= 256;
            }

            return result;
        }

        private static void ProcessInitializationStep(string step, Dictionary<int, List<Lens>> boxes)
        {
            string label = Regex.Match(step, "[a-z]+").Value;
            int labelHash = CalculateHash(label);

            if (!boxes.TryGetValue(labelHash, out var box))
            {
                box = new List<Lens>();
                boxes[labelHash] = box;
            }
            
            if (step.EndsWith('-'))
            {
                RemoveLens(box, label);
            }
            else
            {
                int focalLength = int.Parse(Regex.Match(step, "[0-9]+").Value);
                InsertOrReplaceLens(box, label, focalLength);
            }
        }

        private static void InsertOrReplaceLens(List<Lens> box, string label, int newFocalLength)
        {
            var firstLensWithLabel = box.FirstOrDefault(l => l.Label == label);
            if (firstLensWithLabel is null)
            {
                box.Add(new Lens { Label = label, FocalLength = newFocalLength });
            }
            else
            {
                firstLensWithLabel.FocalLength = newFocalLength;
            }
        }

        private static void RemoveLens(List<Lens> box, string label)
        {
            var firstLensWithLabel = box.FirstOrDefault(l => l.Label == label);
            if (firstLensWithLabel is not null)
            {
                box.Remove(firstLensWithLabel);
            }
        }

        private static int CalculateFocusingPower(Dictionary<int, List<Lens>> boxes)
        {
            int result = 0;

            foreach ((int boxNumber, List<Lens> box) in boxes)
            {
                for (int lensNumber = 0; lensNumber < box.Count; lensNumber++)
                {
                    var lens = box[lensNumber];
                    result += (boxNumber + 1) * (lensNumber + 1) * lens.FocalLength;
                }
            }

            return result;
        }

        private static string[] GetLines(string fileName)
        {
            return File.ReadAllLines(fileName);
        }

        public class Lens()
        {
            public string Label { get; set; } = "";
            public int FocalLength { get; set; }
        }
    }
}
