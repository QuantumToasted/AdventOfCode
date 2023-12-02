namespace AdventOfCode;

public sealed class CampCleanup() : AdventOfCodeChallenge("Camp Cleanup", 2022, 04)
{
    private IReadOnlyCollection<(CleanupAssignment FirstElf, CleanupAssignment SecondElf)> _elfPairs = null!;

    public override void LoadData()
    {
        _elfPairs = _input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(x =>
            {
                var pair = x.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                var firstElf = pair[0].Split('-');
                var secondElf = pair[1].Split('-');
                return (
                    new CleanupAssignment(int.Parse(firstElf[0]), int.Parse(firstElf[1])),
                    new CleanupAssignment(int.Parse(secondElf[0]), int.Parse(secondElf[1]))
                    );
            }).ToList();
    }

    public override string SolvePart1()
    {
        var containedPairCount = 0;
        foreach (var pair in _elfPairs)
        {
            if (pair.FirstElf.Contains(pair.SecondElf) || pair.SecondElf.Contains(pair.FirstElf))
                containedPairCount++;
        }

        return $"The number of elf pairs whose area contains another is {containedPairCount}.";
    }

    public override string SolvePart2()
    {
        var overlappingPairCount = 0;
        foreach (var pair in _elfPairs)
        {
            if (pair.FirstElf.Overlaps(pair.SecondElf))
                overlappingPairCount++;
        }

        return $"The number of elf pairs whose area overlaps is {overlappingPairCount}.";
    }

    private sealed record CleanupAssignment(int StartingSectionId, int EndingSectionId)
    {
        public int[] AssignedSections => Enumerable.Range(StartingSectionId, (EndingSectionId - StartingSectionId) + 1).Select(x => x).ToArray();

        public bool Contains(CleanupAssignment other)
            => other.StartingSectionId >= StartingSectionId && other.EndingSectionId <= EndingSectionId;

        public bool Overlaps(CleanupAssignment other)
            => AssignedSections.Intersect(other.AssignedSections).Any();
    }
}