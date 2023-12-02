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
        var validGames = _games.Where(game => game.IsPart1Valid);

        return $"The sum of valid games is {validGames.Sum(x => x.Id)}.";
    }

    public override string SolvePart2()
        => "TODO";

    private sealed record CubeGameResult
    {
        public CubeGameResult(ReadOnlySpan<char> rawValue)
        {
            var diceStartIndex = 0;
            var id = 0;
            for (var i = 0; i < rawValue.Length; i++)
            {
                var c = rawValue[i];
                if (c == ':')
                {
                    diceStartIndex = i + 2;
                    break;
                }
                
                var digit = (int)char.GetNumericValue(rawValue[i]);
                if (digit == -1)
                    continue;

                id = id * 10 + digit;
            }

            Id = id;
            
            var indexedValue = rawValue[diceStartIndex..];
            Span<Range> setRanges = stackalloc Range[indexedValue.Count(";") + 1];
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

        public bool IsPart1Valid
        {
            get
            {
                foreach (var set in Sets)
                {
                    switch (set.RedCubes, set.GreenCubes, set.BlueCubes)
                    {
                        case (> 12, _, _):
                        case (_, > 13, _):
                        case (_, _, > 14):
                            Print();
                            return false;
                    }
                }

                return true;
            }
        }

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
            var cubeCount = 0;
            for (var i = 0; i < rawValue.Length; i++)
            {
                var c = rawValue[i];

                switch (c)
                {
                    case 'r':
                        RedCubes = cubeCount;
                        cubeCount = 0;
                        i += 4; // 'e', 'd', ' '
                        continue;
                    case 'g':
                        GreenCubes = cubeCount;
                        cubeCount = 0;
                        i += 6; // 'r', 'e', 'e', 'n', ' '
                        continue;
                    case 'b':
                        BlueCubes = cubeCount;
                        cubeCount = 0;
                        i += 5; // 'l', 'u', 'e', ' '
                        continue;
                    case var _ when (int) char.GetNumericValue(c) is not -1 and var digit:
                        cubeCount = cubeCount * 10 + digit;
                        continue;
                }
            }
        }
        
        public int BlueCubes { get; }
        
        public int RedCubes { get; }
        
        public int GreenCubes { get; }
    }
}