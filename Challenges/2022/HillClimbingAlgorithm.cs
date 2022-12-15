using System.Drawing;
using AdventOfCode.Common;

namespace AdventOfCode.Challenges;

[Challenge("Hill Climbing Algorithm", 2022, 12)]
public sealed class HillClimbingAlgorithm : AdventOfCodeChallenge
{
    private Graph<HeightPoint> _map = null!;
    
    public override void LoadData()
    {
        var split = _input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var arr = new HeightPoint[split.Length, split[0].Length];
        
        for (var x = 0; x < split.Length; x++)
        {
            var line = split[x];
            for (var y = 0; y < line.Length; y++)
            {
                arr[x, y] = new HeightPoint(line[y], new Point(x, y));
            }
        }

        _map = new Graph<HeightPoint>(arr);
    }

    public override string SolvePart1()
    {
        var s = _map.Single(x => x.Height == 'S');
        var e = _map.Single(x => x.Height == 'E');

        var paths = new HashSet<Path>();

        var path = new Path(new[] {s}.ToList());

        paths.Add(path);

        _map.Print(true);
        
        Traverse(path);

        foreach (var p in paths.Where(x => x.VisitedPoints[^1].Equals(e)))
        {
            _map.Print(true, p.VisitedPoints.Select(x => (x.Location, ConsoleColor.Green)).ToArray());
            Console.WriteLine($"Length: {p.Length}");
            Console.ReadKey();
        }

        return $"Shortest path is {paths.Where(x => x.VisitedPoints[^1].Equals(e)).MinBy(x => x.Length).Length} steps.";
        
        void Traverse(Path currentPath)
        {
            var current = currentPath.VisitedPoints[^1];
            
            var neighbors = _map.GetCartesianNeighbors(current, p => current.CanTravelTo(p.Height));

            foreach (var neighbor in neighbors)
            {
                var p = new Path(currentPath.VisitedPoints.Append(neighbor).ToList());
                
                if (neighbor.Equals(s)) // special case. don't go right back to start idiot
                    continue;

                // problem line #1

                var currentPathLength = p.Length;
                if (paths.Any(x =>
                    {
                        var length = x.StepsTo(neighbor);
                        // is there a shorter or equal path to this neighbor?
                        return length > 0 && length <= currentPathLength;
                    }))
                {
                    continue;
                }
                
                paths.Add(p);
                
                if (neighbor.Equals(e))
                {
                    return;
                }

                // problem line #2?
                // currentPath.VisitedPoints.Add(neighbor);

                
                //Console.SetCursorPosition(position.Left, position.Top);
                //Console.WriteLine(list.Count);
                //var tempPosition = Console.GetCursorPosition();
                //Console.WriteLine("\n\n\n\n\n\n\n\n\n\n");
                //Console.SetCursorPosition(tempPosition.Left, tempPosition.Top);
                //Console.Write(currentPath);

                _map.PrintFast(currentPath.VisitedPoints.Select(x => (x.Location, ConsoleColor.Red)).ToArray());

                Traverse(p);
            }
        }
    }

    public override string SolvePart2()
    {
        return string.Empty;
    }

    private readonly record struct Path(List<HeightPoint> VisitedPoints)
    {
        //public List<HeightPoint> VisitedPoints { get; } = new();

        public int Length => VisitedPoints.Distinct().Count() - 1;// VisitedPoints.Count - 1; // don't include the starting point

        public int StepsTo(HeightPoint point)
         => VisitedPoints.IndexOf(point)/* + 1*/; // no + 1, see Length
        
        public bool Equals(Path other)
            => other.VisitedPoints.SequenceEqual(VisitedPoints);

        public override int GetHashCode()
            => VisitedPoints.GetHashCode();

        public override string ToString()
            => new(VisitedPoints.Select(x => x.Height).ToArray());
    }

    private readonly record struct HeightPoint(char Height, Point Location)
    {
        public bool IsTheTop => Height == 'E';
        
        // can travel to a point at most 1 height greater than the current point,
        // but can travel to a point of any height LESS than the current point
        public bool CanTravelTo(char otherHeight)
        {
            if (otherHeight == 'E')
                return Height == 'z';

            if (otherHeight is 'a' or 'b' && Height == 'S')
                return true;

            return otherHeight - Height <= 1;
        }

        public bool Equals(HeightPoint? other)
            => other?.Location.Equals(Location) == true;

        public override int GetHashCode()
            => Location.GetHashCode();

        public override string ToString()
            => Height.ToString();

        public static implicit operator Point(HeightPoint p) => p.Location;
    }
}