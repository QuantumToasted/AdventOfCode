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

                if (!isAdjacentToSymbol && _graph.GetAllNeighbors(p, (_, x) => !char.IsDigit(x) && x != '.').Length > 0)
                {
                    isAdjacentToSymbol = true;
                }
            }

            if (isAdjacentToSymbol)
                sum += partNumber;
            
            digits.Clear();
        }
        
        return $"The sum of all part numbers in the engine schematic is {sum}.";
    }

    public override string SolvePart2()
    {
        var potentialGearLocations = _graph.Where(x => x.Value == '*').Select(x => x.Point).ToDictionary(x => x, _ => new List<int>());
        
        var digits = new List<(Point Point, int Digit)>();
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

            Point? gearLocation = null;
            foreach (var (p, d) in digits)
            {
                partNumber = partNumber * 10 + d;

                _graph.GetAllNeighbors(p, (l, x) =>
                {
                    if (potentialGearLocations.ContainsKey(l))
                    {
                        gearLocation = l;
                        return !char.IsDigit(x) && x != '.';
                    }

                    return false;
                });
            }

            if (gearLocation.HasValue)
            {
                var list = potentialGearLocations[gearLocation.Value];
                list.Add(partNumber);
            }
            
            digits.Clear();
        }

        var sum = potentialGearLocations.Values.Where(x => x.Count == 2).Sum(x => x.Aggregate(1L, (l, r) => l * r));
        return $"The sum of all gear ratios in the engine schematic is {sum}.";
    }
}