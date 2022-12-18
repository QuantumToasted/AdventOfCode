using System.Collections;
using System.Text;
using AdventOfCode.Common;

namespace AdventOfCode.Challenges;

[Challenge("Distress Signal", 2022, 13)]
public sealed class DistressSignal : AdventOfCodeChallenge
{
    private ICollection<(IPacketValue Left, IPacketValue Right)> _part1PacketPairs = null!;
    private ICollection<IPacketValue> _part2SortedPackets = null!;

    public override void LoadData()
    {
        _part1PacketPairs = _input.Split("\n\n", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(pair => pair.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            .Select(lines => (Parse(lines[0]), Parse(lines[1]))).ToList();

        _part2SortedPackets = _input.Replace("\n\n", "\n")
            .Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(Parse)
            .Append(Parse("[[2]]"))
            .Append(Parse("[[6]]"))
            .OrderDescending()
            .ToList();
    }

    public override string SolvePart1()
    {
        var index = 1;
        var sum = 0;

        foreach (var (left, right) in _part1PacketPairs)
        {
            if (left.CompareTo(right) == 1)
                sum += index;
            
            index++;
        }

        return $"The sum of all indices of correctly ordered packet pairs is {sum}.";
    }

    public override string SolvePart2()
    {
        var formattedPackets = _part2SortedPackets.Select(x => x.Format()).ToList();
        var dividerPacket1Index = formattedPackets.IndexOf("[[2]]") + 1;
        var dividerPacket2Index = formattedPackets.IndexOf("[[6]]") + 1;

        return $"The decoder key for the distress signal is {dividerPacket1Index * dividerPacket2Index}.";
    }

    // Future me note:
    // IComparable<IPacketValue>.CompareTo() ALWAYS ASSUMES it is always comparing a "left" packet to a "right" packet.
    private interface IPacketValue : IComparable<IPacketValue>
    {
        string Format();
    }

    private readonly record struct IntegerPacketValue(int Value) : IPacketValue
    {
        public string Format()
            => Value.ToString();

        public override string ToString()
            => Format();

        // 1 = in order, -1 = out of order
        public int CompareTo(IPacketValue? other)
        {
            return other switch
            {
                IntegerPacketValue i => -Value.CompareTo(i.Value), // invert the result from int.CompareTo(int)
                ListPacketValue l => new ListPacketValue(new IPacketValue[] {this}).CompareTo(l),
                _ => throw new ArgumentOutOfRangeException(nameof(other))
            };
        }
    }

    private readonly record struct ListPacketValue(ICollection<IPacketValue> Values) : IPacketValue
    {
        public string Format()
            => $"[{string.Join(',', Values.Select(x => x.Format()))}]";
        
        public override string ToString()
            => Format();

        public int CompareTo(IPacketValue? other)
        {
            if (other is IntegerPacketValue i)
                return CompareTo(new ListPacketValue(new IPacketValue[] { i }));

            if (other is not ListPacketValue right)
                throw new ArgumentOutOfRangeException(nameof(other));

            var leftQueue = new Queue<IPacketValue>(Values);
            var rightQueue = new Queue<IPacketValue>(right.Values);

            while (leftQueue.TryDequeue(out var leftItem))
            {
                if (!rightQueue.TryDequeue(out var rightItem))
                {
                    return -1;
                }

                var compareResult = leftItem.CompareTo(rightItem);
                if (compareResult != 0)
                    return compareResult;
            }

            if (rightQueue.Count > 0)
                return 1;

            return 0;
        }
    }

    private static IPacketValue Parse(string input)
    {
        var value = new ListPacketValue(new List<IPacketValue>());

        var innerValueStack = new Stack<IPacketValue>(new IPacketValue[] { value });

        var builder = new StringBuilder();
        foreach (var c in input[1..^1]) // read from inside [ ]
        {
            var currentValue = innerValueStack.Peek();

            if (c is ',')
            {
                if (builder.Length > 0 && currentValue is ListPacketValue list)
                {
                    list.Values.Add(new IntegerPacketValue(int.Parse(builder.ToString())));
                    builder.Clear();
                }

                continue;
            }

            switch (c)
            {
                case '[' when currentValue is ListPacketValue list:
                    var newListValue = new ListPacketValue(new List<IPacketValue>());
                    list.Values.Add(newListValue);
                    innerValueStack.Push(newListValue);
                    break;
                case ']' when currentValue is ListPacketValue list && builder.Length > 0:
                    list.Values.Add(new IntegerPacketValue(int.Parse(builder.ToString())));
                    builder.Clear();
                    innerValueStack.Pop();
                    break;
                case ']':
                    innerValueStack.Pop();
                    break;
                default:
                    builder.Append(c);
                    break;
            }
        }

        return value;
    }
}