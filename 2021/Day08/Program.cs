// #define Sample

{
#if Sample
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day08\Sample.txt";
#else
    string fileName = @"D:\Dropbox\Work\AdventOfCode\2021\Day08\Full.txt";
#endif

    string[] input = File.ReadAllLines(fileName);
    Entry[] entries = input.Select(s => new Entry(s)).ToArray();

    Console.WriteLine("Part 1: " + Part1(entries));
    Console.WriteLine("Part 2: " + Part2(entries));
    Console.ReadLine();
}

long Part1(Entry[] entries)
{
    return entries.SelectMany(e => e.Output).Count(e => e.Length is 2 or 4 or 3 or 7);
}

long Part2(Entry[] entries)
{
    return entries.Sum(e => e.GetOutputValue());
}

public class Entry
{
    public Entry(string line)
    {
        string[] parts = line.Split('|').ToArray();
        Input = parts[0].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToArray();
        Output = parts[1].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToArray();
    }

    public string[] Input;
    public string[] Output;

    Dictionary<char, char> TranslationsRealToWrong = [];
    Dictionary<char, char> TranslationsWrongToReal = [];

    private void DetermineTranslation() 
    {
        if (TranslationsRealToWrong.Count > 0) return;

        //  We know that input contains every possible digit. So we can use them all.
        //
        //  Length of word to digits:
        //  2: 1
        //  3: 7
        //  4: 4
        //  5: 2,3,5,
        //  6: 0,6,9
        //  7: 8
        //
        //  Algorithm:
        //  a is disctinction between 1(2er) word and 7(3er) word->Knowing a
        //  g is common between 0,6,9(6er) words except 4(4er) word and except a-> Knowing a, g
        //  d is common between 2,3,5(4er) words except known a, g -> Knowing a, g, d
        //  b is 4(4er word) except 1(2er) word and excpept known d->Knowing a,g,d,b
        //  f is common between 0,6,9(6er) words except known a, b, d, g  -> Knowing a, g, d, b, f
        //  c is 4(er) word without known b, d, f -> Knowing a, g, d, b, f, c
        //  e is 8(7er) word without known a, g, d, b, f, c

        string oneWord = Input.Where(s => s.Length == 2).Single();
        string sevenWord = Input.Where(s => s.Length == 3).Single();
        string fourWord = Input.Where(s => s.Length == 4).Single();
        string eightWord = Input.Where(s => s.Length == 7).Single();
        string[] twoThreeFiveWords = Input.Where(s => s.Length == 5).ToArray();
        string[] zeroSixNineWords = Input.Where(s => s.Length == 6).ToArray();

        // a is disctinction between 1(2er) word and 7(3er) word
        TranslationsRealToWrong['a'] = 
            sevenWord
            .Except(oneWord)
            .Single();

        // g is common between 0,6,9 (6er) words except 4 (4er) word and except a
        TranslationsRealToWrong['g'] =
            zeroSixNineWords.Aggregate(Intersect)
            .Except(fourWord)
            .Except(TranslationsRealToWrong.Values)
            .Single();

        // d is common between 2,3,5 (4er) words except known a,g
        TranslationsRealToWrong['d'] =
            twoThreeFiveWords.Aggregate(Intersect)
            .Except(TranslationsRealToWrong.Values)
            .Single();

        // b is 4 (4er word) except 1 (2er) word and excpept known d
        TranslationsRealToWrong['b'] =
            fourWord
            .Except(oneWord)
            .Except(TranslationsRealToWrong.Values)
            .Single();

        // f is common between 0,6,9 (6er) words except known a,b,d,g
        TranslationsRealToWrong['f'] =
            zeroSixNineWords.Aggregate(Intersect)
            .Except(TranslationsRealToWrong.Values)
            .Single();

        // c is 4 (er) word without known b,d,f
        TranslationsRealToWrong['c'] =
            fourWord
            .Except(TranslationsRealToWrong.Values)
            .Single();

        // e is 8 (7er) word without known a,g,d,b,f,c
        TranslationsRealToWrong['e'] =
            eightWord
            .Except(TranslationsRealToWrong.Values)
            .Single();

        TranslationsWrongToReal = TranslationsRealToWrong.ToDictionary(p => p.Value, p => p.Key);
    }

    private static string Intersect(string a, string b)
    {
        return  new string(a.Intersect(b).ToArray());
    }

    public int GetOutputValue()
    {
        DetermineTranslation();

        int result = 0;
        foreach (var word in Output)
        {
            result *= 10;
            result += TranslateWord(word);
        }

        return result;
    }

    private int TranslateWord(string word)
    {
        string sortedAndTranslated = 
            new string(
                word.Select(ch => TranslationsWrongToReal[ch])
                .Order()
                .ToArray());

        return sortedAndTranslated switch
        {

            "abcefg" => 0,
            "cf" => 1,
            "acdeg" => 2,
            "acdfg" => 3,
            "bcdf" => 4,
            "abdfg" => 5,
            "abdefg" => 6,
            "acf" => 7,
            "abcdefg" => 8,
            "abcdfg" => 9,
            _ => throw new ApplicationException("unknown word")
        };
    }
}