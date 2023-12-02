using System.Drawing;

namespace AdventOfCode;

public sealed class RopeBridge() : AdventOfCodeChallenge("Rope Bridge", 2022, 09)
{
    private ICollection<RopeHeadMovement> _movements = null!;

    public override void LoadData()
    {
        _movements = _input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(RopeHeadMovement.Parse)
            .ToList();
    }

    public override string SolvePart1()
    {
        var ropeSections = new RopeSection[2]; // head, "tail"
        var visitedPoints = new HashSet<Point>();
        
        foreach (var movement in _movements)
        {
            for (var i = movement.Amount; i > 0; i--)
            {
                for (var r = 0; r < ropeSections.Length; r++)
                {
                    var section = ropeSections[r];

                    if (r == 0) // head
                    {
                        ropeSections[r] = movement.Direction switch
                        {
                            RopeMoveDirection.Up => section with { Y = section.Y + 1},
                            RopeMoveDirection.Down => section with { Y = section.Y - 1},
                            RopeMoveDirection.Right => section with { X = section.X + 1},
                            RopeMoveDirection.Left => section with { X = section.X - 1},
                            _ => throw new ArgumentOutOfRangeException()
                        };
                    }
                    else
                    {
                        ropeSections[r] = section.Follow(ropeSections[r - 1]);
                    }
                }

                visitedPoints.Add(ropeSections.Last());
            }
        }

        return $"The tail of the rope visited {visitedPoints.Count} unique points.";
    }

    public override string SolvePart2()
    {
        var ropeSections = new RopeSection[10]; // head + 9 sections
        var visitedPoints = new HashSet<Point>();
        
        foreach (var movement in _movements)
        {
            for (var i = movement.Amount; i > 0; i--)
            {
                for (var r = 0; r < ropeSections.Length; r++)
                {
                    var section = ropeSections[r];

                    if (r == 0) // head
                    {
                        ropeSections[r] = movement.Direction switch
                        {
                            RopeMoveDirection.Up => section with { Y = section.Y + 1},
                            RopeMoveDirection.Down => section with { Y = section.Y - 1},
                            RopeMoveDirection.Right => section with { X = section.X + 1},
                            RopeMoveDirection.Left => section with { X = section.X - 1},
                            _ => throw new ArgumentOutOfRangeException()
                        };
                    }
                    else
                    {
                        ropeSections[r] = section.Follow(ropeSections[r - 1]);
                    }
                }

                visitedPoints.Add(ropeSections.Last());
            }
        }

        return $"The tail of the rope visited {visitedPoints.Count} unique points.";
    }

    private sealed record RopeSection(int X, int Y)
    {
        public RopeSection Follow(RopeSection head)
        {
            if (head.X == X && head.Y == Y)
                return this;

            var xDistance = X - head.X;
            var xAbsoluteDistance = Math.Abs(xDistance);

            var yDistance = Y - head.Y;
            var yAbsoluteDistance = Math.Abs(yDistance);

            if (xAbsoluteDistance < 2 && yAbsoluteDistance < 2)
                return this;
            if (xDistance == 0 && yAbsoluteDistance > 1)
                return this with { Y = Y - Math.Sign(yDistance) };
            if (yDistance == 0 && xAbsoluteDistance > 1)
                return this with { X = X - Math.Sign(xDistance) };
            return this with { X = X - Math.Sign(xDistance), Y = Y - Math.Sign(yDistance) };
        }

        public static implicit operator Point(RopeSection section) => new(section.X, section.Y);
    }

    private sealed record RopeHeadMovement(RopeMoveDirection Direction, int Amount)
    {
        public static RopeHeadMovement Parse(string input)
        {
            var split = input.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            return new RopeHeadMovement(split[0][0] switch
            {
                'U' => RopeMoveDirection.Up,
                'D' => RopeMoveDirection.Down,
                'R' => RopeMoveDirection.Right,
                'L' => RopeMoveDirection.Left,
                _ => throw new ArgumentOutOfRangeException()
            }, int.Parse(split[1]));
        }
    }

    private enum RopeMoveDirection
    {
        Up,
        Down,
        Right,
        Left
    }
}