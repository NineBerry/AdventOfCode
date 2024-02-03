// #define Sample

using System.Text;

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day08\Sample.txt";
    int imageWidth = 3;
    int imageHeight = 2;
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day08\Full.txt";
    int imageWidth = 25;
    int imageHeight = 6;
#endif

    string input = File.ReadAllText(fileName);
    Layer[] layers = Layer.CreateLayers(input, imageWidth, imageHeight);

    Console.WriteLine("Part 1: " + Part1(layers));
    Console.WriteLine("Part 2: " + Part2(layers));
    Console.ReadLine();
}

long Part1(Layer[] layers)
{
    Layer layer = layers.OrderBy(l => l.CountDigit('0')).First();
    return layer.CountDigit('1') * layer.CountDigit('2');
}

string Part2(Layer[] layers)
{
    Layer layer = Layer.RenderLayers(layers);
    return Environment.NewLine + layer.ToString(); ;
}

public class Layer
{
    public string[] Image;

    public int CountDigit(char digit)
    {
        return Image.SelectMany(s => s).Count(ch => ch == digit );
    }

    public static Layer[] CreateLayers(string input, int imageWidth, int imageHeight)
    {
        List<Layer> layers = [];
        int index = 0;

        while(index < input.Length)
        {
            List<string> buffer = [];
            for(int i = 0; i < imageHeight; i++) 
            {
                buffer.Add(input.Substring(index, imageWidth));
                index += imageWidth;
            }
            layers.Add(new Layer { Image = buffer.ToArray()});
        }

        return layers.ToArray();
    }

    public static Layer RenderLayers(Layer[] layers)
    {
        int imageWidth = layers[0].Image[0].Length;
        int imageHeight = layers[0].Image.Length;

        var image = Enumerable.Range(0, imageHeight).Select(e => new StringBuilder(new string(' ', imageWidth))).ToArray();

        for(int x=0; x < imageWidth; x++)
        {
            for (int y = 0; y < imageHeight; y++)
            {
                image[y][x] = IsPixelBlack(layers, x, y) ? ' ' : '█';
            }
        }

        return new Layer { Image = image.Select(sb => sb.ToString()).ToArray() };
    }

    private static bool IsPixelBlack(Layer[] layers, int x, int y)
    {
        foreach (Layer layer in layers)
        {
            char ch = layer.Image[y][x];
            if (ch == '2') continue;

            return ch == '0';
        }

        return false;
    }

    public override string ToString()
    {
        return string.Join(Environment.NewLine, Image);
    }
}