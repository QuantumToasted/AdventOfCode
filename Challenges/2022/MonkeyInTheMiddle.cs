namespace AdventOfCode;

public sealed class MonkeyInTheMiddle() : AdventOfCodeChallenge("Monkey in the Middle", 2022, 11)
{
    private IList<Monkey> _monkeys = null!;
    
    public override void LoadData()
    {
        _monkeys = _input.Split("\n\n", StringSplitOptions.None | StringSplitOptions.RemoveEmptyEntries)
            .Select(Monkey.Parse)
            .ToList();
    }

    public override string SolvePart1()
    {
        const int roundCount = 20;
        
        for (var round = 1; round <= roundCount; round++)
        {
            foreach (var monkey in _monkeys)
            {
                while (monkey.Items.TryDequeue(out var item))
                {
                    monkey.ItemsInspected++;
                    
                    // increase worry from monkey inspecting item
                    item = monkey.Operation(item);
                    
                    // divide worry by 3 due to monkey getting bored
                    item /= 3;

                    // divisor test: true -> throw to TrueMonkeyIndex, false -> throw to FalseMonkeyIndex
                    if (item % monkey.Divisor == 0)
                    {
                        _monkeys[monkey.TrueMonkeyIndex].Items.Enqueue(item);
                    }
                    else
                    {
                        _monkeys[monkey.FalseMonkeyIndex].Items.Enqueue(item);
                    }
                }
            }
        }

        var monkeyBusinessLevel = _monkeys.OrderByDescending(x => x.ItemsInspected).Take(2)
            .Aggregate(1, (i, monkey) => i * monkey.ItemsInspected);

        return $"The level of monkey business after {roundCount} rounds is {monkeyBusinessLevel}.";
    }

    public override string SolvePart2()
    {
        // re-load data to reset items and items inspected
        LoadData();
        
        const int roundCount = 10_000;

        // the limit is the LCM of all monkeys' divisors (which just so happen to be prime)
        // the current item (worry level) can be modulo'd with the lcm to keep it within sane limits
        var lcm = _monkeys.Aggregate(1, (i, monkey) => i * monkey.Divisor);
        
        for (var round = 1; round <= roundCount; round++)
        {
            foreach (var monkey in _monkeys)
            {
                while (monkey.Items.TryDequeue(out var item))
                {
                    monkey.ItemsInspected++;
                    
                    // increase worry from monkey inspecting item
                    item = monkey.Operation(item);
                    
                    // don't divide worry by 3 due to monkey getting bored, anymore
                    // item /= 3;

                    // modulo the item with the least common multiple to keep it sane
                    item %= lcm;

                    // divisor test: true -> throw to TrueMonkeyIndex, false -> throw to FalseMonkeyIndex
                    if (item % monkey.Divisor == 0)
                    {
                        _monkeys[monkey.TrueMonkeyIndex].Items.Enqueue(item);
                    }
                    else
                    {
                        _monkeys[monkey.FalseMonkeyIndex].Items.Enqueue(item);
                    }
                }
            }
        }

        var monkeyBusinessLevel = _monkeys.OrderByDescending(x => x.ItemsInspected).Take(2)
            .Aggregate(1L, (i, monkey) => checked(i * monkey.ItemsInspected));
        
        return $"The level of monkey business after {roundCount} rounds is {monkeyBusinessLevel}.";
    }
    
    private sealed record Monkey(Queue<long> Items, Func<long, long> Operation, int Divisor, int TrueMonkeyIndex, int FalseMonkeyIndex)
    {
        public int ItemsInspected { get; set; }

        public static Monkey Parse(string input)
        {
            var split = input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[1..];
            var items = split[0].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[2..]
                .Select(x => long.Parse(x.TrimEnd(',')));

            Func<long, long> operation;
            var operationSplit = split[1].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var @operator = operationSplit[4][0];
            if (int.TryParse(operationSplit[5], out var operationNumber))
            {
                operation = @operator switch
                {
                    '*' => i => i * operationNumber,
                    '+' => i => i + operationNumber,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            else
            {
                operation = @operator switch
                {
                    '*' => i => i * i,
                    '+' => i => i + i,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            var divisor = int.Parse(split[2].Split(' ')[3]);

            var trueMonkeyIndex = int.Parse(split[3].Split(' ')[5]);

            var falseMonkeyIndex = int.Parse(split[4].Split(' ')[5]);

            return new Monkey(new Queue<long>(items), operation, divisor, trueMonkeyIndex, falseMonkeyIndex);
        }
    }
}