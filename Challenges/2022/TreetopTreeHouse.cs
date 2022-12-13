using System.Drawing;
using AdventOfCode.Common;
using Serilog;

namespace AdventOfCode.Challenges;

[Challenge("Treetop Tree House", 2022, 08)]
public sealed class TreetopTreeHouse : AdventOfCodeChallenge
{
    private Graph<Tree> _trees = null!;

    public override void LoadData()
    {
        var lines = _input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var arr = new Tree[lines.Length, lines[0].Length];
        for (var x = 0; x < lines.Length; x++)
        {
            var line = lines[x];
            for (var y = 0; y < line.Length; y++)
            {
                arr[x, y] = new Tree(int.Parse(line[y].ToString()), new Point(x, y));
            }
        }

        _trees = new Graph<Tree>(arr);
    }

    public override string SolvePart1()
    {
        var visibleTrees = _trees.Count(x => x.IsVisibleFromOutside(_trees));
        return $"The number of trees fully visible from the outside is {visibleTrees}.";
    }

    public override string SolvePart2()
    {
        var maximumScenicScore = _trees.MaxBy(x => x.GetScenicScore(_trees));
        
        return $"The maximum scenic score among all trees is the tree located at {maximumScenicScore.Position} with {maximumScenicScore.GetScenicScore(_trees)}.";
    }

    private readonly record struct Tree(int Height, Point Position)
    {
        public bool IsVisibleFromOutside(Graph<Tree> trees)
        {
            if (Position.X == 0 || Position.Y == 0)
                return true;
            
            var @this = this;
            
            var ySlice = trees[@this.Position.Y, GraphDimension.Y];

            // up
            if (ySlice.Where(x => x.Position.X < @this.Position.X).All(x => x.Height < @this.Height))
                return true;
            
            // down
            if (ySlice.Where(x => x.Position.X > @this.Position.X).All(x => x.Height < @this.Height))
                return true;

            var xSlice = trees[@this.Position.X, GraphDimension.X];
            
            // right
            if (xSlice.Where(x => x.Position.Y < @this.Position.Y ).All(x => x.Height < @this.Height))
                return true;

            // left
            if (xSlice.Where(x => x.Position.Y > @this.Position.Y).All(x => x.Height < @this.Height))
                return true;

            return false;
        }

        public int GetScenicScore(Graph<Tree> trees)
        {
            // right
            var rightScore = 0;
            for (var y = Position.Y - 1; y >= 0; y--)
            {
                rightScore++;
                
                var tree = trees[Position.X, y];
                if (tree.Height >= Height)
                    break;
            }
            
            // left
            var leftScore = 0;
            for (var y = Position.Y + 1; y < trees.GetLength(GraphDimension.Y); y++)
            {
                leftScore++;
                
                var tree = trees[Position.X, y];
                if (tree.Height >= Height)
                    break;
            }
            
            // up
            var upScore = 0;
            for (var x = Position.X - 1; x >= 0; x--)
            {
                upScore++;

                var tree = trees[x, Position.Y];
                if (tree.Height >= Height)
                    break;
            }

            // down
            var downScore = 0;
            for (var x = Position.X + 1; x < trees.GetLength(GraphDimension.X); x++)
            {
                downScore++;
                
                var tree = trees[x, Position.Y];
                if (tree.Height >= Height)
                    break;
            }

            if (Height == 9)
            {
                
            }

            var totalScore = rightScore * leftScore * upScore * downScore;

            if (totalScore > 100_000)
            {
                Log.Debug("Tree @ {Position} with height {Height}", Position, Height);
                Log.Debug("R: {RightScore}, L: {LeftScore}, U: {UpScore}, D: {DownScore}", rightScore, leftScore, upScore, downScore);
                Log.Debug("Total: {Score}", totalScore);
            }
            
            return totalScore;
        }
        
        public static implicit operator int(Tree tree) => tree.Height;
    }
}