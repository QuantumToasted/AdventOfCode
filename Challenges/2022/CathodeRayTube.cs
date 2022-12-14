using System.Drawing;
using System.Text;
using AdventOfCode.Common;
using Serilog;

namespace AdventOfCode.Challenges;

[Challenge("Cathode-Ray Tube", 2022, 10)]
public sealed class CathodeRayTube : AdventOfCodeChallenge
{
    private ICollection<Instruction> _instructions = null!;

    public override void LoadData()
    {
        _instructions = _input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(Instruction.Parse)
            .ToList();
    }

    public override string SolvePart1()
    {
        var cycleCount = 0;
        var xRegister = 1;

        var signalStrengthSum = 0;
        
        foreach (var instruction in _instructions)
        {
            do
            {
                cycleCount++;

                if (cycleCount is 20 or 60 or 100 or 140 or 180 or 220)
                {
                    signalStrengthSum += cycleCount * xRegister;
                }
                    
                    
            } while (!instruction.TryComplete(ref xRegister));
        }

        return $"The sum of signal strengths from cycles 20, 60, 100, 140, 180, & 220th cycles is {signalStrengthSum}.";
    }

    public override string SolvePart2()
    {
        const int xMax = 40;
        const int yMax = 6;
        
        var cycleCount = 0;
        var xRegister = 1;

        var crt = new Graph<char>(new char[xMax, yMax]);
        
        foreach (var instruction in _instructions)
        {
            do
            {
                if (cycleCount >= crt.TotalLength)
                    break;
                
                var beamPosition = ToPoint(cycleCount);
                var spritePosition = ToPoint(xRegister);
                
                if (beamPosition.X == spritePosition.X ||
                    beamPosition.X == Math.Max(0, spritePosition.X - 1) ||
                    beamPosition.X == Math.Min(xMax - 1, spritePosition.X + 1))
                {
                    crt[beamPosition.X, beamPosition.Y] = '#';
                }
                else
                {
                    crt[beamPosition.X, beamPosition.Y] = '.';
                }
                
                cycleCount++;
                
            } while (!instruction.TryComplete(ref xRegister));
        }

        var solutionBuilder = new StringBuilder().AppendLine();
        for (var i = 0; i < yMax; i++)
        {
            var row = crt[i, GraphDimension.X];
            solutionBuilder.AppendLine(new string(row));
        }

        return solutionBuilder.ToString();

        static Point ToPoint(int value)
            => new(value % 40, value / 40);
    }

    private abstract class Instruction
    {
        public abstract bool TryComplete(ref int xRegister);

        public static Instruction Parse(string input)
        {
            var split = input.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            return split[0] switch
            {
                "noop" => new NoOpInstruction(),
                "addx" => new AddInstruction { Amount = int.Parse(split[1]) },
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    private sealed class NoOpInstruction : Instruction
    {
        public override bool TryComplete(ref int xRegister) => true;
    }

    private sealed class AddInstruction : Instruction
    {
        private bool _isComplete;
        
        public int Amount { get; init; }
        
        public override bool TryComplete(ref int xRegister)
        {
            if (!_isComplete)
            {
                _isComplete = true;
                return false;
            }
            
            xRegister += Amount;
            _isComplete = false;
            return true;
        }
    }
}