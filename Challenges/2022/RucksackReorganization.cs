using AdventOfCode.Common;

namespace AdventOfCode.Challenges;

[Challenge("Rucksack Reorganization", 2022, 03)]
public sealed class RucksackReorganization : AdventOfCodeChallenge
{
    private IReadOnlyCollection<Rucksack> _part1Rucksacks = null!;
    private IReadOnlyCollection<Rucksack[]> _part2RucksackGroups = null!;

    public override void LoadData()
    {
        _part1Rucksacks = _input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(x => new Rucksack(x))
            .ToList();
        
        _part2RucksackGroups = _input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(x => new Rucksack(x))
            .Chunk(3)
            .ToList();
    }

    public override string SolvePart1()
    {
        var prioritySum = _part1Rucksacks.Sum(x => x.CommonItemPriority);
        return $"The sum of all rucksacks' common item priority is {prioritySum}.";
    }

    public override string SolvePart2()
    {
        var prioritySum = 0;
        foreach (var group in _part2RucksackGroups)
        {
            foreach (var item in Rucksack.PRIORITY_STRING)
            {
                if (group.All(x => x.FullContents.Contains(item)))
                {
                    prioritySum += Rucksack.PRIORITY_STRING.IndexOf(item) + 1;
                    break;
                }
            }
        }

        return $"The sum of each group's common item priority is {prioritySum}.";
    }

    private readonly record struct Rucksack(string FullContents)
    {
        public const string PRIORITY_STRING = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public string FirstCompartmentContents => FullContents[..(FullContents.Length / 2)];
        public string SecondCompartmentContents => FullContents[(FullContents.Length / 2)..];
        public char CommonItem
        {
            get
            {
                var @this = this;
                return @this.FirstCompartmentContents.First(x => @this.SecondCompartmentContents.Contains(x));
            }
        }
        public int CommonItemPriority => PRIORITY_STRING.IndexOf(CommonItem) + 1;
    }
}