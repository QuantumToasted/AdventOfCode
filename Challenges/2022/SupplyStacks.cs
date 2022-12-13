using AdventOfCode.Common;
using Serilog;

namespace AdventOfCode.Challenges;

[Challenge("Supply Stacks", 2022, 05)]
public sealed class SupplyStacks : AdventOfCodeChallenge
{
    private IReadOnlyList<Stack<char>> _crateStacks = null!;
    private IReadOnlyCollection<SingleCrateMoveInstruction> _part1Instructions = null!;
    private IReadOnlyCollection<MultiCrateMoveInstruction> _part2Instructions = null!;

    public override void LoadData()
    {
        var split = _input.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);

        var rawStacks = split[0].Split('\n').Reverse().Skip(1).ToArray();
        var stacks = new List<Stack<char>>();
        foreach (var line in rawStacks)
        {
            var chunks = line.Chunk(4).Select(x => new string(x)).ToList();
            if (stacks.Count == 0)
            {
                stacks = Enumerable.Range(1, chunks.Count).Select(x => new Stack<char>()).ToList();
            }
            
            for (var i = 0; i < chunks.Count; i++)
            {
                var chunk = chunks[i];
                if (!string.IsNullOrWhiteSpace(chunks[i]))
                    stacks[i].Push(chunk[1]);
            }
        }

        foreach (var stack in stacks)
        {
            Log.Information("Crates: {Crates}", stack);
        }

        _crateStacks = stacks;
        
        _part1Instructions = split[1]
            .Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(SingleCrateMoveInstruction.Parse).ToList();
        
        _part2Instructions = split[1]
            .Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(MultiCrateMoveInstruction.Parse).ToList();
    }

    public override string SolvePart1()
    {
        foreach (var instruction in _part1Instructions)
        {
            instruction.Apply(ref _crateStacks);
        }

        var topCrates = string.Concat(_crateStacks.Select(x => x.Peek()));
        return $"The top crates in each stack spell out {topCrates}.";
    }

    public override string SolvePart2()
    {
        LoadData(); // re-load data
        
        foreach (var instruction in _part2Instructions)
        {
            instruction.Apply(ref _crateStacks);
        }

        var topCrates = string.Concat(_crateStacks.Select(x => x.Peek()));
        return $"The top crates in each stack spell out {topCrates}.";
    }

    private readonly record struct SingleCrateMoveInstruction(int Count, int From, int To)
    {
        public void Apply(ref IReadOnlyList<Stack<char>> crateStacks)
        {
            // instructions From and To are 1-indexed, but the list is 0-indexed
            for (var i = 0; i < Count; i++)
            {
                var crate = crateStacks[From - 1].Pop();
                crateStacks[To - 1].Push(crate);
            }
        }

        public static SingleCrateMoveInstruction Parse(string input)
        {
            var split = input.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            return new SingleCrateMoveInstruction(int.Parse(split[1]), int.Parse(split[3]), int.Parse(split[5]));
        }
    }
    
    private readonly record struct MultiCrateMoveInstruction(int Count, int From, int To)
    {
        public void Apply(ref IReadOnlyList<Stack<char>> crateStacks)
        {
            // instructions From and To are 1-indexed, but the list is 0-indexed
            var temporaryStack = new Stack<char>();
            
            for (var i = 0; i < Count; i++)
            {
                var crate = crateStacks[From - 1].Pop();
                temporaryStack.Push(crate);
            }
            
            for (var i = 0; i < Count; i++)
            {
                var crate = temporaryStack.Pop();
                crateStacks[To - 1].Push(crate);
            }
        }

        public static MultiCrateMoveInstruction Parse(string input)
        {
            var split = input.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            return new MultiCrateMoveInstruction(int.Parse(split[1]), int.Parse(split[3]), int.Parse(split[5]));
        }
    }
}