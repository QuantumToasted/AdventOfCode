using System.Reflection;

namespace AdventOfCode.Common;

public abstract class AdventOfCodeChallenge
{
    public static readonly IReadOnlyDictionary<DateOnly, Type> ExistingChallenges = typeof(AdventOfCodeChallenge).Assembly.GetTypes()
        .Where(x => !x.IsAbstract && typeof(AdventOfCodeChallenge).IsAssignableFrom(x))
        .ToDictionary(x =>
        {
            var attribute = x.GetCustomAttribute<ChallengeAttribute>()!;
            return new DateOnly(attribute.Year, 12, attribute.Day);
        });
    
    private protected readonly string _input;

    protected AdventOfCodeChallenge()
    {
        var attribute = GetType().GetCustomAttribute<ChallengeAttribute>()!;
        
        _input = File.ReadAllText($"Inputs/{attribute.Year}/day{attribute.Day:00}.txt");

        Year = attribute.Year;
        Day = attribute.Day;
        Name = attribute.Name;
    }
    
    public int Year { get; }
    
    public int Day { get; }

    public string Name { get; }

    public abstract void LoadData();

    public abstract string SolvePart1();

    public abstract string SolvePart2();

    public static AdventOfCodeChallenge? Today
    {
        get
        {
            var now = DateTimeOffset.Now;
            return Find(now.Year, now.Day);
        }
    }

    public static AdventOfCodeChallenge? Latest
    {
        get
        {
            var highestYear = ExistingChallenges.Keys.MaxBy(x => x.Year).Year;
            var highestYearDay = ExistingChallenges.Keys.Where(x => x.Year == highestYear).MaxBy(x => x.Day).Day;
            return Find(highestYear, highestYearDay);
        }
    }

    public static AdventOfCodeChallenge? CurrentYear(int day)
        => Find(DateTimeOffset.Now.Year, day);

    public static AdventOfCodeChallenge? Find(int year, int day)
    {
        if (!ExistingChallenges.TryGetValue(new DateOnly(year, 12, day), out var type)
            || Activator.CreateInstance(type) is not AdventOfCodeChallenge challenge)
        {
            return null;
        }

        return challenge;
    }
}