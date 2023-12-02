using System.Drawing;
using System.Text;

namespace AdventOfCode;

public sealed class RegolithReservoir() : AdventOfCodeChallenge("Regolith Reservoir", 2022, 14)
{
    private static readonly Point SandInitialPosition = new(500, 0);
    
    private Cave _cave = null!;
    
    public override void LoadData()
    {
        var formations = _input
            .Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(CaveRockFormation.Parse)
            .ToList();

        _cave = new Cave();

        foreach (var formation in formations)
        {
            formation.Form(_cave);
        }
    }

    public override string SolvePart1()
    {
        Point? restingPosition;
        
        do
        {
            restingPosition = _cave.CalculateRestingPosition(SandInitialPosition);

            if (restingPosition is { } position)
            {
                _cave.CollisionMap[position] = Material.Sand;
            }

        } while (restingPosition is not null);

        var sandAtRest = _cave.CollisionMap.Values.Count(x => x is Material.Sand);
        return $"The number of grains of sand which come to rest before all sand falls endlessly is {sandAtRest}.";
    }

    public override string SolvePart2()
    {
        // empty the cave
        LoadData();
        
        var floorY = _cave.CollisionMap.Max(x => x.Key.Y) + 2;
        var floor = new CaveRockFormation(new[] { new Point(0, floorY), new Point(1000, floorY) });
        floor.Form(_cave);

        var targetPosition = new Point(500, 0);
        
        Point? restingPosition;
        
        do
        {
            restingPosition = _cave.CalculateRestingPosition(SandInitialPosition);

            if (restingPosition is { } position)
            {
                _cave.CollisionMap[position] = Material.Sand;
            }

        } while (restingPosition != targetPosition);

        var sandAtRest = _cave.CollisionMap.Values.Count(x => x is Material.Sand);
        return $"The number of grains of sand which come to rest before the source is plugged is {sandAtRest}.";
    }

    private sealed record Cave
    {
        public Dictionary<Point, Material> CollisionMap { get; } = new();
        
        public Point? CalculateRestingPosition(Point sandPosition)
        {
            //Print(sandPosition);
            //Thread.Sleep(500);
            
            var maxY = CollisionMap.Keys.Max(x => x.Y);

            if (sandPosition.Y > maxY)
                return null; // will fall forever

            // A unit of sand always falls down one step if possible.
            if (CollisionMap.ContainsKey(sandPosition with { Y = sandPosition.Y + 1 }))
            {
                // If the tile immediately below is blocked (by rock or sand),
                // the unit of sand attempts to instead move diagonally one step down and to the left.
                if (CollisionMap.ContainsKey(sandPosition with { Y = sandPosition.Y + 1, X = sandPosition.X - 1 }))
                {
                    // If that tile is blocked,
                    // the unit of sand attempts to instead move diagonally one step down and to the right
                    if (CollisionMap.ContainsKey(sandPosition with { Y = sandPosition.Y + 1, X = sandPosition.X + 1 }))
                    {
                        // If all three possible destinations are blocked,
                        // the unit of sand comes to rest and no longer moves
                        return sandPosition;
                    }
                    
                    return CalculateRestingPosition(sandPosition with { Y = sandPosition.Y + 1, X = sandPosition.X + 1 });
                }
                
                return CalculateRestingPosition(sandPosition with { Y = sandPosition.Y + 1, X = sandPosition.X - 1 });
            }

            // Sand keeps moving as long as it is able to do so
            return CalculateRestingPosition(sandPosition with { Y = sandPosition.Y + 1 });
        }

        public void Print(Point sandPosition)
        {
            var maxX = CollisionMap.Max(x => x.Key.X);
            var minX = CollisionMap.Min(x => x.Key.X);
            
            var maxY = CollisionMap.Max(x => x.Key.Y);
            var minY = CollisionMap.Min(x => x.Key.Y);

            var graph = new Graph<char>(new char[maxX - minX + 1, maxY + 1]);
            graph.Fill('.');

            foreach (var (point, material) in CollisionMap)
            {
                graph[point.X - minX, point.Y] = material switch
                {
                    Material.Air => '.',
                    Material.Rock => '#',
                    Material.Sand => 'o',
                    Material.FallingSand => '+',
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            var builder = new StringBuilder();
            
            for (var y = 0; y < graph.GetLength(GraphDimension.Y); y++)
            {
                for (var x = 0; x < graph.GetLength(GraphDimension.X); x++)
                {
                    builder.Append(graph[x, y]);
                }
                
                builder.AppendLine();
            }
            
            Console.SetCursorPosition(0, 0);
            Console.WriteLine(builder.ToString());
            Console.SetCursorPosition(sandPosition.X - minX, sandPosition.Y);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write('+');
            Console.ResetColor();
            Console.SetCursorPosition(sandPosition.X - minX, Math.Max(0, sandPosition.Y - 40));
            
            //File.WriteAllText($"{Guid.NewGuid():N}.txt", builder.ToString());
        }
    }

    private sealed record CaveRockFormation(Point[] DrawPoints)
    {
        public void Form(Cave cave)
        {
            for (var i = 1; i < DrawPoints.Length; i++)
            {
                var currentPoint = DrawPoints[i - 1];
                var targetPoint = DrawPoints[i];
                
                var xDistance = targetPoint.X - currentPoint.X;
                var yDistance = targetPoint.Y - currentPoint.Y;
                
                cave.CollisionMap[currentPoint] = Material.Rock;

                do
                {
                    if (xDistance == 0)
                        currentPoint.Y += Math.Sign(yDistance);
                    else
                        currentPoint.X += Math.Sign(xDistance);
                    
                    cave.CollisionMap[currentPoint] = Material.Rock;
                } 
                while (currentPoint != targetPoint);
            }
        }

        public static CaveRockFormation Parse(string input)
        {
            return new CaveRockFormation(input
                .Split("->", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                .Select(x => new Point(int.Parse(x[0]), int.Parse(x[1]))).ToArray());
        }
    }

    private enum Material
    {
        Air,
        Rock,
        Sand,
        FallingSand
    }
}