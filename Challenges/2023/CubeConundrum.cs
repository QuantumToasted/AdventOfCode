namespace AdventOfCode;

public sealed class CubeConundrum() : AdventOfCodeChallenge("Cube Conundrum", 2023, 02)
{
    private ICollection<CubeGameResult> _games = null!;
    
    public override void LoadData()
    {
        _games = _input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(x => new CubeGameResult(x))
            .ToList();
    }

    public override string SolvePart1()
    {
        var validGames = _games.Where(game =>
        {
            foreach (var set in game.Sets)
            {
                switch (set.RedCubes, set.GreenCubes, set.BlueCubes)
                {
                    case (> 12, _, _):
                    case (_, > 13, _):
                    case (_, _, > 14):
                        game.Print();
                        return false;
                }
            }

            return true;
        });

        return $"The sum of valid games is {validGames.Sum(x => x.Id)}.";
    }

    public override string SolvePart2()
        => "TODO";

    private sealed record CubeGameResult
    {
        public CubeGameResult(ReadOnlySpan<char> rawValue)
        {
            var diceStartIndex = 0;
            for (var i = 0; i < rawValue.Length; i++)
            {
                var id = (int)char.GetNumericValue(rawValue[i]);
                if (id == -1)
                    continue;

                diceStartIndex = i + 3;

                var nextDigit = (int)char.GetNumericValue(rawValue[i + 1]);
                if (nextDigit != -1)
                {
                    id = id * 10 + nextDigit;
                    diceStartIndex += 1;
                }

                Id = id;
                break;
            }
            
            var indexedValue = rawValue[diceStartIndex..];
            Span<Range> setRanges = stackalloc Range[indexedValue.Count(";")];
            var setCount = indexedValue.Split(setRanges, ';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            if (setCount <= 0)
                throw new Exception();

            var sets = new List<CubeGameSet>();

            for (var i = 0; i < setCount; i++)
            {
                sets.Add(new CubeGameSet(indexedValue[setRanges[i]]));
            }

            Sets = sets;
        }
        
        public int Id { get; }
        
        public IReadOnlyList<CubeGameSet> Sets { get; }

        public void Print()
        {
            Console.Write($"Game {Id}: ");

            for (var i = 0; i < Sets.Count; i++)
            {
                var set = Sets[i];
                if (set.RedCubes > 0)
                {
                    if (set.RedCubes > 12)
                        Console.ForegroundColor = ConsoleColor.Red;

                    Console.Write($"{set.RedCubes} red");
                    Console.ResetColor();

                    if (set.GreenCubes > 0 || set.BlueCubes > 0)
                        Console.Write(", ");
                }

                if (set.GreenCubes > 0)
                {
                    if (set.GreenCubes > 13)
                        Console.ForegroundColor = ConsoleColor.Red;

                    Console.Write($"{set.GreenCubes} green");
                    Console.ResetColor();

                    if (set.BlueCubes > 0)
                        Console.Write(", ");
                }

                if (set.BlueCubes > 0)
                {
                    if (set.BlueCubes > 14)
                        Console.ForegroundColor = ConsoleColor.Red;

                    Console.Write($"{set.BlueCubes} blue");
                    Console.ResetColor();
                }
                
                if (i < Sets.Count - 1)
                    Console.Write("; ");
            }

            Console.WriteLine();
        }
    }

    private sealed record CubeGameSet
    {
        public CubeGameSet(ReadOnlySpan<char> rawValue)
        {
            for (var i = 0; i < rawValue.Length; i++)
            {
                var cubeCount = (int)char.GetNumericValue(rawValue[i]);
                if (cubeCount == -1)
                    continue;

                var nextDigit = (int)char.GetNumericValue(rawValue[i + 1]);
                if (nextDigit != -1)
                {
                    i++;
                    cubeCount = cubeCount * 10 + nextDigit;
                }

                switch (rawValue[i + 2])
                {
                    case 'b': // blue
                        BlueCubes = cubeCount;
                        continue;
                    case 'r': // red
                        RedCubes = cubeCount;
                        continue;
                    case 'g': // green
                        GreenCubes = cubeCount;
                        continue;
                }
            }
        }
        
        public int BlueCubes { get; }
        
        public int RedCubes { get; }
        
        public int GreenCubes { get; }
    }
}