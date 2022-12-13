using AdventOfCode.Common;

namespace AdventOfCode.Challenges;

[Challenge("Calorie Counting", 2022, 01)]
public sealed class CalorieCounting : AdventOfCodeChallenge
{
    private IReadOnlyCollection<ElfCalorieCollection> _elves = null!;

    public override void LoadData()
    {
        var elves = _input.Split("\n\n");
        _elves = elves.Select(x => new ElfCalorieCollection(x.Split('\n',
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray()))
            .ToList();
    }

    public override string SolvePart1()
    {
        var maxCaloriesElf = _elves.MaxBy(x => x.Sum)!;
        return $"The elf with the most calories is carrying {maxCaloriesElf.Sum} calories worth of food.";
    }

    public override string SolvePart2()
    {
        var maxCalorieElves = _elves.OrderByDescending(x => x.Sum).Take(3);
        return $"The three elves with the most calories are carrying a combined {maxCalorieElves.Sum(x => x.Sum)} calories worth of food.";
    }

    private readonly record struct ElfCalorieCollection(int[] CalorieGroups)
    {
        public int Sum => CalorieGroups.Sum(x => x);
    }
}