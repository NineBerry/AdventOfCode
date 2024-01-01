using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Day02
{
    internal class Program
    {
        // private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day02\Sample.txt";
        private static string fileName = @"D:\Dropbox\Work\AdventOfCode\2023\Day02\Full.txt";


        public class RGB
        {
            public int R = 0;
            public int G = 0;
            public int B = 0;

            public RGB(int r, int g, int b) 
            { 
                this.R = r;
                this.G = g;
                this.B = b;
            }
            public RGB(string s)
            {
                this.R = int.Parse("0" + Regex.Match(s, @"(\d+) red").Groups[1].Value);
                this.G = int.Parse("0" + Regex.Match(s, @"(\d+) green").Groups[1].Value);
                this.B = int.Parse("0" + Regex.Match(s, @"(\d+) blue").Groups[1].Value);
            }

            public bool FitsIn(RGB other)
            {
                return (this.R <= other.R && 
                    this.G <= other.G &&
                    this.B <= other.B);
            }

            public int Power()
            {
                return R * G * B;
            }
        }

        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(fileName);

            Console.WriteLine("Part 1: " + lines.Sum(ProcessLinePart1));
            Console.WriteLine("Part 2: " + lines.Sum(ProcessLinePart2));
            Console.ReadLine();
        }

        private static int ProcessLinePart1(string line)
        {
            int id = int.Parse(Regex.Match(line, @"\d+").Value);
            line = Regex.Replace(line, ".*:", "");

            var drawTexts = line.Split(";", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var draws = drawTexts.Select(s => new RGB(s));

            RGB limits = new RGB(12, 13, 14);
            return draws.All(draw => draw.FitsIn(limits)) ? id : 0;
        }

        private static int ProcessLinePart2(string line)
        {
            line = Regex.Replace(line, ".*:", "");

            var drawTexts = line.Split(";", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var draws = drawTexts.Select(s => new RGB(s));

            int minR = (from d in draws select d.R).Max();
            int minG = (from d in draws select d.G).Max();
            int minB = (from d in draws select d.B).Max();

            RGB min = new RGB(minR, minG, minB);

            int power = min.Power();

            return power;
        }
    }
}
