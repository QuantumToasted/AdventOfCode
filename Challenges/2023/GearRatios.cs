using System.Drawing;

namespace AdventOfCode;

public sealed class GearRatios() : AdventOfCodeChallenge("Gear Ratios", 2023, 03)
{
    private Graph<char> _graph = null!;
    
    public override void LoadData()
    {
        var lines = _input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var arr = new char[lines.Length, lines[0].Length];

        for (var x = 0; x < lines.Length; x++)
        {
            var line = lines[x];
            for (var y = 0; y < line.Length; y++)
            {
                arr[x, y] = line[y];
            }
        }
        
        _graph = new Graph<char>(arr);
    }

    public override string SolvePart1()
    {
        var sum = 0;

        var digits = new List<(Point Point, int Digit)>();
        var points = new List<Point>();
        foreach (var (point, c) in _graph)
        {
            var digit = (int)char.GetNumericValue(c);
            if (digit != -1)
            {
                digits.Add((point, digit));
                
                if (point.Y < _graph.GetLength(GraphDimension.Y) - 1)
                    continue;
            }

            if (digits.Count == 0)
                continue;

            var partNumber = 0;
            var isAdjacentToSymbol = false;
            
            foreach (var (p, d) in digits)
            {
                partNumber = partNumber * 10 + d;
                
                /*
                if (!isAdjacentToSymbol)
                {
                    var neighbors = _graph.GetAllNeighbors(p);

                    foreach (var neighbor in neighbors)
                    {
                        if (!char.IsDigit(neighbor) && neighbor != '.')
                        {
                            isAdjacentToSymbol = true;
                        }
                    }
                }
                */

                if (!isAdjacentToSymbol && _graph.GetAllNeighbors(p, x => !char.IsDigit(x) && x != '.').Length > 0)
                {
                    isAdjacentToSymbol = true;
                    points.AddRange(digits.Select(x => x.Point));
                }
            }

            if (isAdjacentToSymbol)
                sum += partNumber;
            
            digits.Clear();
        }

        //_graph.Print(true, points.Select(x => (x, ConsoleColor.Red)).ToArray());        
        
        return $"The sum of all part numbers in the engine schematic is {sum}.";
    }

    public override string SolvePart2()
        => "TODO";
}