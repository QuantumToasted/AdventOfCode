using AdventOfCode.Common;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Is(args.FirstOrDefault()?.Equals("debug", StringComparison.CurrentCultureIgnoreCase) == true
        ? LogEventLevel.Debug
        : LogEventLevel.Information)
    .WriteTo.Console()
    .CreateLogger();

AdventOfCodeChallenge? challenge;

if (AdventOfCodeChallenge.ExistingChallenges.Count == 0)
{
    Log.Fatal("No available challenges.");
    Console.ReadLine();
    Environment.Exit(-1);
}

Log.Information("Available challenges: ");
foreach (var yearGroup in AdventOfCodeChallenge.ExistingChallenges.Keys.OrderByDescending(x => x.Year).GroupBy(x => x.Year))
{
    Log.Information("{Year}: {Days}", yearGroup.Key, 
        string.Join(", ", yearGroup.Select(x => x.Day).Order().Select(x => x.ToString("00"))));
}

Log.Information("Valid inputs: \"[t]oday\", \"[l]atest\", \"2023 03\", \"03\"");
Log.Information("Default (no input): latest");

var input = Console.ReadLine()?.ToLower();
if (string.IsNullOrWhiteSpace(input) || input is "l" or "latest")
{
    challenge = AdventOfCodeChallenge.Latest;
    Log.Information("Selected \"latest\" challenge.");
}
else if (input is "t" or "today")
{
    challenge = AdventOfCodeChallenge.Today;
    Log.Information("Selected \"today\" challenge.");
}
else
{
    var split = input.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    if (split.Length == 2 && int.TryParse(split[0], out var year) && int.TryParse(split[1], out var day))
    {
        challenge = AdventOfCodeChallenge.Find(year, day);
    }
    else if (int.TryParse(split[0], out day))
    {
        year = DateTimeOffset.Now.Year;
        challenge = AdventOfCodeChallenge.CurrentYear(day);
    }
    else
    {
        Log.Warning("Invalid input. Defaulting to \"latest\" challenge.");
        challenge = AdventOfCodeChallenge.Latest;
        year = challenge?.Year ?? DateTimeOffset.Now.Year;
        day = challenge?.Day ?? DateTimeOffset.Now.Day;
    }
    
    Log.Information("Selected challenge from year {Year}, day {Day}.", year, day);
}

if (challenge is null)
{
    if (string.IsNullOrWhiteSpace(input))
    {
        Log.Fatal("No challenges have been created!");
    }
    else
    {
        Log.Fatal("No challenge could be found with the criteria \"{Input}\"!", input);
    }
    
    Console.ReadLine();
    Environment.Exit(-1);
}

try
{
    Log.Information("Loading data for challenge {Challenge}:", challenge.Name);
    challenge.LoadData();
    Log.Information("Done!");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Failed to load data!");
    Console.ReadLine();
    Environment.Exit(-1);
}

Log.Information("Running challenge {Challenge}:", challenge.Name);

try
{
    var solution = challenge.SolvePart1();
    Log.Information("[PART 1]");
    Log.Information("Solution: {Solution}", solution);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Failed to run part 1!");
    Console.ReadLine();
    Environment.Exit(-1);
}

try
{
    var solution = challenge.SolvePart2();
    Log.Information("[PART 2]");
    Log.Information("Solution: {Solution}", solution);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Failed to run part 2!");
    Console.ReadLine();
    Environment.Exit(-1);
}

Console.ReadLine();
