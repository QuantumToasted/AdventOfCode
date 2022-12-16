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

        var queue = new Queue<HeightPoint>(new[] {s});

        var previous = new Dictionary<HeightPoint, HeightPoint>();

        while (queue.TryDequeue(out var point))
        {
            foreach (var neighbor in _map.GetCartesianNeighbors(point, x => point.CanTravelTo(x.Height)))
            {
                if (previous.ContainsKey(neighbor))
                    continue;

                previous[neighbor] = point;
                queue.Enqueue(neighbor);
            }
        }

        var path = new Path(new List<HeightPoint>());
        var current = e;
        while (!current.Equals(s))
        {
            path.VisitedPoints.Add(current);
            current = previous[current];
        }
        
        path.VisitedPoints.Add(s);

        path.VisitedPoints.Reverse();

        _map.Print(true, path.VisitedPoints.Select(x => (x.Location, ConsoleColor.Green)).ToArray());

        return $"The shortest possible path from S to E is {path.Length} steps.";
    }

    public override string SolvePart2()
    {
        var e = _map.Single(x => x.Height == 'E');

        var paths = new List<Path>();

        foreach (var p in _map.Where(x => x.Height is 'a' or 'S'))
        {
            var queue = new Queue<HeightPoint>(new[] {p});
            var previous = new Dictionary<HeightPoint, HeightPoint>();
            while (queue.TryDequeue(out var point))
            {
                foreach (var neighbor in _map.GetCartesianNeighbors(point, x => point.CanTravelTo(x.Height)))
                {
                    if (previous.ContainsKey(neighbor))
                        continue;

                    previous[neighbor] = point;
                    queue.Enqueue(neighbor);
                }
            }
            
            // skip to the next point if the path DOES NOT lead to the summit
            if (!previous.ContainsKey(e))
                continue;

            var path = new Path(new List<HeightPoint>());
            var current = e;
            while (!current.Equals(p))
            {
                path.VisitedPoints.Add(current);
                current = previous[current];
            }
        
            path.VisitedPoints.Add(p);
            path.VisitedPoints.Reverse();
            
            paths.Add(path);
        }

        var shortestPath = paths.MinBy(x => x.Length);

        _map.Print(true, shortestPath.VisitedPoints.Select(x => (x.Location, ConsoleColor.Green)).ToArray());

        return $"The shortest possible path from any a-height point to E is {shortestPath.Length} steps.";
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
                return Height is 'z' or 'y';

            if (otherHeight is 'a' or 'b' && Height is 'S')
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