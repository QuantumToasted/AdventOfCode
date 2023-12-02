namespace AdventOfCode;

public abstract class AdventOfCodeChallenge(string name, int year, int day)
{
    public static readonly IReadOnlyDictionary<DateOnly, AdventOfCodeChallenge> ExistingChallenges = typeof(AdventOfCodeChallenge).Assembly.GetTypes()
        .Where(x => !x.IsAbstract && typeof(AdventOfCodeChallenge).IsAssignableFrom(x))
        .Select(x => (AdventOfCodeChallenge)Activator.CreateInstance(x)!)
        .ToDictionary(x => x.Date);
    
    private protected readonly string _input = File.ReadAllText($"Inputs/{year}/day{day:00}.txt");

    public string Name { get; } = name;
    
    public DateOnly Date { get; } = new(year, 12, day);

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
        => ExistingChallenges.GetValueOrDefault(new DateOnly(year, 12, day));
}