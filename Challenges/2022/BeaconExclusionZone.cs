using System.Collections.Concurrent;
using System.Drawing;
using System.Numerics;
using System.Text.RegularExpressions;
using AdventOfCode.Common;
using Serilog;

namespace AdventOfCode.Challenges;

[Challenge("Beacon Exclusion Zone", 2022, 15)]
public sealed class BeaconExclusionZone : AdventOfCodeChallenge
{
    private ICollection<Sensor> _sensors = null!;

    public override void LoadData()
    {
        _sensors = _input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(Sensor.Parse)
            .ToList();
    }

    public override string SolvePart1()
    {
        return "Skipping.";
        const int yCheck = 2_000_000;

        var nonBeaconPointCount = 0;
        for (var x = -10_000_000; x < 10_000_000; x++)
        {
            var point = new Point(x, yCheck);

            foreach (var sensor in _sensors)
            {
                var sensorMaxDistance = ManhattanDistance(sensor.Position, sensor.NearestBeaconPosition);

                if (point == sensor.Position || point == sensor.NearestBeaconPosition)
                    continue;
                
                var distance = ManhattanDistance(sensor.Position, point);

                if (distance <= sensorMaxDistance)
                {
                    nonBeaconPointCount++;
                    break;
                }
            }
        }

        return $"The number of positions at y={yCheck} that cannot contain a beacon is {nonBeaconPointCount}.";
    }

    public override string SolvePart2()
    {
        const int max = 4_000_000;
        // const int max = 20;

        Point? beaconLocation = null;

        var totalRowsTried = new ConcurrentBag<long>();

        Parallel.For(0L, max, (x, loopState) =>
        {
            totalRowsTried.Add(x);
            
            Log.Information("Current X: {X} ({Count} remaining)", x, max - totalRowsTried.Count);

            var range = new List<int[]>();
            foreach (var sensor in _sensors)
            {
                var beaconDistance = ManhattanDistance(sensor.Position, sensor.NearestBeaconPosition);
                var rowDistance = (int) Math.Abs(x - sensor.Position.X);
                var difference = Math.Abs(beaconDistance - rowDistance);
                range.Add(Enumerable.Range(beaconDistance - difference, difference * 2 + 1).ToArray());
            }

            var fullRange = range.SelectMany(a => a).Where(i => i is >= 0 and <= max).ToHashSet();
            if (fullRange.Count == max)
                return;

            for (var y = 0; !fullRange.Contains(y) && y < max; y++)
            {
                var point = new Point((int) x, y);
                if (_sensors.All(sensor =>
                    {
                        var sensorMaxDistance = ManhattanDistance(sensor.Position, sensor.NearestBeaconPosition);
                        if (point == sensor.Position || point == sensor.NearestBeaconPosition)
                            return false;

                        var distance = ManhattanDistance(sensor.Position, point);
                        return distance > sensorMaxDistance;
                    }))
                {
                    beaconLocation = point;
                    loopState.Break();
                }
            }
        });

        if (beaconLocation.HasValue)
        {
            long tuningFrequency = beaconLocation.Value.X * 4_000_000 + beaconLocation.Value.Y;
            return $"The point at {beaconLocation.Value} must be a beacon - its tuning frequency is {tuningFrequency}.";
        }
        
        return "No point found.";
    }

    private static int ManhattanDistance(Point point1, Point point2)
        => Math.Abs(point1.X - point2.X) + Math.Abs(point1.Y - point2.Y);

    private readonly record struct Sensor(Point Position, Point NearestBeaconPosition)
    {
        private static readonly Regex SensorParseRegex = new(
            @"Sensor at x=(-?[\d]+), y=(-?[\d]+): closest beacon is at x=(-?[\d]+), y=(-?[\d]+)", RegexOptions.Compiled);
        
        public static Sensor Parse(string input)
        {
            var match = SensorParseRegex.Match(input);
            return new Sensor(
                new Point(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value)),
                new Point(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value)));
        }
    }
}