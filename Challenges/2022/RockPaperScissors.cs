using AdventOfCode.Common;

namespace AdventOfCode.Challenges;

[Challenge("Rock Paper Scissors", 2022, 02)]
public sealed class RockPaperScissors : AdventOfCodeChallenge
{
    private IReadOnlyCollection<RockPaperScissorsOutcome> _part1Outcomes = null!;
    private IReadOnlyCollection<RockPaperScissorsOutcome> _part2Outcomes = null!;

    public override void LoadData()
    {
        _part1Outcomes = _input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(x =>
            {
                var split = x.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                return RockPaperScissorsOutcome.ParseThrows(split[0][0], split[1][0]);
            })
            .ToList();
        
        _part2Outcomes = _input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(x =>
            {
                var split = x.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                return RockPaperScissorsOutcome.ParseThrowWithOutcome(split[0][0], split[1][0]);
            })
            .ToList();
    }

    public override string SolvePart1()
    {
        var totalScore = _part1Outcomes.Sum(x => x.ScoreEarned);
        return $"Following the original strategy guide, your total score earned would be {totalScore} points.";
    }

    public override string SolvePart2()
    {
        var totalScore = _part2Outcomes.Sum(x => x.ScoreEarned);
        return $"Following the updated strategy guide, your total score earned would be {totalScore} points.";
    }
    
    private enum RockPaperScissorsThrow : int
    {
        Rock = 1,
        Paper = 2,
        Scissors = 3
    }

    private readonly record struct RockPaperScissorsOutcome(RockPaperScissorsThrow ElfThrow, RockPaperScissorsThrow PlayerThrow)
    {
        public int ScoreEarned
        {
            get
            {
                var throwScore = (int) PlayerThrow;
                int outcomeScore = PlayerThrow switch
                {
                    RockPaperScissorsThrow.Rock when ElfThrow is RockPaperScissorsThrow.Scissors => 6,
                    RockPaperScissorsThrow.Paper when ElfThrow is RockPaperScissorsThrow.Rock => 6,
                    RockPaperScissorsThrow.Scissors when ElfThrow is RockPaperScissorsThrow.Paper => 6,
                    _ when ElfThrow == PlayerThrow => 3,
                    _ => 0
                };

                return throwScore + outcomeScore;
            }
        }

        public static RockPaperScissorsOutcome ParseThrows(char elfThrow, char playerThrow)
        {
            return new RockPaperScissorsOutcome(
                elfThrow switch
                {
                    'A' => RockPaperScissorsThrow.Rock,
                    'B' => RockPaperScissorsThrow.Paper,
                    'C' => RockPaperScissorsThrow.Scissors,
                    _ => throw new ArgumentOutOfRangeException()
                },
                playerThrow switch
                {
                    'X' => RockPaperScissorsThrow.Rock,
                    'Y' => RockPaperScissorsThrow.Paper,
                    'Z' => RockPaperScissorsThrow.Scissors,
                    _ => throw new ArgumentOutOfRangeException()
                }
            );
        }
        
        public static RockPaperScissorsOutcome ParseThrowWithOutcome(char elfThrow, char outcome)
        {
            return new RockPaperScissorsOutcome(
                elfThrow switch
                {
                    'A' => RockPaperScissorsThrow.Rock,
                    'B' => RockPaperScissorsThrow.Paper,
                    'C' => RockPaperScissorsThrow.Scissors,
                    _ => throw new ArgumentOutOfRangeException()
                },
                outcome switch
                {
                    'X' when elfThrow is 'A' => RockPaperScissorsThrow.Scissors,
                    'X' when elfThrow is 'B' => RockPaperScissorsThrow.Rock,
                    'X' when elfThrow is 'C' => RockPaperScissorsThrow.Paper,
                    'Y' when elfThrow is 'A' => RockPaperScissorsThrow.Rock,
                    'Y' when elfThrow is 'B' => RockPaperScissorsThrow.Paper,
                    'Y' when elfThrow is 'C' => RockPaperScissorsThrow.Scissors,
                    'Z' when elfThrow is 'A' => RockPaperScissorsThrow.Paper,
                    'Z' when elfThrow is 'B' => RockPaperScissorsThrow.Scissors,
                    'Z' when elfThrow is 'C' => RockPaperScissorsThrow.Rock,
                    _ => throw new ArgumentOutOfRangeException()
                }
            );
        }
    }
}