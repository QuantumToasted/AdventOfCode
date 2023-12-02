namespace AdventOfCode;

public sealed class Trebuchet() : AdventOfCodeChallenge("Trebuchet?!", 2023, 01)
{
    private static readonly IReadOnlyDictionary<string, int> NumberNames = new Dictionary<string, int>
    {
        ["one"] = 1,
        ["two"] = 2,
        ["three"] = 3,
        ["four"] = 4,
        ["five"] = 5,
        ["six"] = 6,
        ["seven"] = 7,
        ["eight"] = 8,
        ["nine"] = 9
    };
    
    private ICollection<CalibrationValue> _calibrationValues = null!;
    
    public override void LoadData()
    {
        _calibrationValues = _input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(CalibrationValue.Parse)
            .ToList();
    }

    public override string SolvePart1()
        => $"The sum of all calibration values is {_calibrationValues.Sum(x => x.Part1Value)}.";

    public override string SolvePart2()
        => $"The sum of all calibration values is {_calibrationValues.Sum(x => x.Part2Value)}.";

    private sealed record CalibrationValue(string RawValue)
    {
        public int Part1Value
        {
            get
            {
                var firstNumber = int.MinValue;
                var lastNumber = int.MinValue;

                for (var i = 0; i < RawValue.Length; i++)
                {
                    var digit = (int) char.GetNumericValue(RawValue[i]);
                    if (digit == -1)
                        continue;

                    if (firstNumber == int.MinValue)
                        firstNumber = digit;

                    lastNumber = digit;
                }

                return firstNumber * 10 + lastNumber; // 5, 3 => 53
            }
        }

        public int Part2Value
        {
            get
            {
                var firstNumber = int.MinValue;
                var lastNumber = int.MinValue;

                for (var i = 0; i < RawValue.Length; i++)
                {
                    var digit = (int) char.GetNumericValue(RawValue[i]);
                    if (digit == -1)
                    {
                        if (!TryReadDigit(RawValue, i, out digit))
                            continue;
                    }

                    if (firstNumber == int.MinValue)
                        firstNumber = digit;

                    lastNumber = digit;
                }

                return firstNumber * 10 + lastNumber; // 5, 3 => 53;

                static bool TryReadDigit(ReadOnlySpan<char> rawValue, int i, out int digit)
                {
                    digit = default;

                    if (char.IsDigit(rawValue[i]))
                        return false;
                    
                    foreach (var (number, value) in NumberNames)
                    {
                        if (i + number.Length > rawValue.Length)
                            continue;

                        var slice = rawValue.Slice(i, number.Length);
                        if (slice.SequenceEqual(number))
                        {
                            digit = value;
                            return true;
                        }
                    }

                    return false;
                }
            }
        }

        public static CalibrationValue Parse(string input)
            => new(input);
    }
}