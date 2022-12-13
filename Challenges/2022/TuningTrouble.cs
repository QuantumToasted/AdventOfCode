using AdventOfCode.Common;

namespace AdventOfCode.Challenges;

[Challenge("Tuning Trouble", 2022, 06)]
public sealed class TuningTrouble : AdventOfCodeChallenge
{
    public override void LoadData()
    { }

    public override string SolvePart1()
    {
        var uniqueSequenceIndex = 0;
        for (var i = 0; i < _input.Length; i++)
        {
            var hashSet = new HashSet<char>(_input[i..(i + 4)]);
            if (hashSet.Count == 4)
            {
                uniqueSequenceIndex = i;
                break;
            }
        }
        
        return $"The index of the first start-of-packet marker is {uniqueSequenceIndex + 4}.";
    }

    public override string SolvePart2()
    {
        var uniqueSequenceIndex = 0;
        for (var i = 0; i < _input.Length; i++)
        {
            var hashSet = new HashSet<char>(_input[i..(i + 14)]);
            if (hashSet.Count == 14)
            {
                uniqueSequenceIndex = i;
                break;
            }
        }
        
        return $"The index of the first start-of-message marker is {uniqueSequenceIndex + 14}.";
    }
}