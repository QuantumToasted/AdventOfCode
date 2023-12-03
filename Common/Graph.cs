using System.Collections;
using System.Drawing;
using System.Text;

namespace AdventOfCode;

public enum GraphDimension
{
    X,
    Y
}

public sealed class Graph<T>(T[,] arr) : IEnumerable<(Point Point, T Value)>
    where T : struct
{
    private readonly T[,] _arr = arr;

    public T this[Point p]
    {
        get => _arr[p.X, p.Y];
        set => _arr[p.X, p.Y] = value;
    }

    public T this[int x, int y]
    {
        get => _arr[x, y];
        set => _arr[x, y] = value;
    }

    // slice
    public T[] this[int index, GraphDimension dimension]
    {
        get
        {
            var length = _arr.GetLength((int)dimension);
            if (index >= length)
                throw new ArgumentOutOfRangeException();

            var arr = new T[length];
            
            var xDimension = dimension == GraphDimension.X;
            
            for (var i = 0; i < length; i++)
            {
                arr[i] = xDimension ? _arr[i, index] : _arr[index, i];
            }

            return arr;
        }
    }

    public T[] GetCartesianNeighbors(Point point, Predicate<T>? predicate = null)
        => GetCartesianNeighbors(point.X, point.Y, predicate);

    public T[] GetCartesianNeighbors(int x, int y, Predicate<T>? predicate = null)
    {
        var neighbors = new List<T>();
        
        if (y > 0)
        {
            var value = _arr[x, y - 1];
            if (predicate?.Invoke(value) != false)
                neighbors.Add(value);
        }

        if (y < _arr.GetLength(1) - 1)
        {
            var value = _arr[x, y + 1];
            if (predicate?.Invoke(value) != false)
                neighbors.Add(value);
        }

        if (x > 0)
        {
            var value = _arr[x - 1, y];
            if (predicate?.Invoke(value) != false)
                neighbors.Add(value);
        }

        if (x < _arr.GetLength(0) - 1)
        {
            var value = _arr[x + 1, y];
            if (predicate?.Invoke(value) != false)
                neighbors.Add(value);
        }

        return neighbors.ToArray();
    }

    public T[] GetAllNeighbors(Point point, Predicate<T>? predicate = null)
        => GetAllNeighbors(point.X, point.Y, predicate);
    
    public T[] GetAllNeighbors(int x, int y, Predicate<T>? predicate = null)
    {
        var neighbors = GetCartesianNeighbors(x, y, predicate).ToList();

        if (x > 0 && y > 0) // top left
        {
            var value = _arr[x - 1, y - 1];
            if (predicate?.Invoke(value) != false)
                neighbors.Add(value);
        }
        
        if (x > 0 && y < _arr.GetLength(1) - 1) // top right
        {
            var value = _arr[x - 1, y + 1];
            if (predicate?.Invoke(value) != false)
                neighbors.Add(value);
        }
        
        if (x < _arr.GetLength(0) - 1 && y > 0) // bottom left
        {
            var value = _arr[x + 1, y - 1];
            if (predicate?.Invoke(value) != false)
                neighbors.Add(value);
        }
        
        if (x < _arr.GetLength(0) - 1 && y < _arr.GetLength(1) - 1) // bottom right
        {
            var value = _arr[x + 1, y + 1];
            if (predicate?.Invoke(value) != false)
                neighbors.Add(value);
        }

        return neighbors.ToArray();
    }

    public int TotalLength => _arr.Length;

    public int GetLength(GraphDimension dimension)
        => _arr.GetLength((int) dimension);

    public void Fill(T value)
    {
        for (var x = 0; x < _arr.GetLength(0); x++)
        {
            for (var y = 0; y < _arr.GetLength(1); y++)
            {
                _arr[x, y] = value;
            }
        }
    }
    
    public void Fill(Func<Point, T> valueFactory)
    {
        for (var x = 0; x < _arr.GetLength(0); x++)
        {
            for (var y = 0; y < _arr.GetLength(1); y++)
            {
                _arr[x, y] = valueFactory(new Point(x, y));
            }
        }
    }

    public void Print(bool writeOver, params (Point Point, ConsoleColor Color)[] highlightedPoints)
    {
        if (writeOver)
        {
            //Console.Clear();
            Console.SetCursorPosition(0, 0);

            var builder = new StringBuilder();
            for (var x = 0; x < _arr.GetLength(0); x++)
            {
                for (var y = 0; y < _arr.GetLength(1); y++)
                {
                    var element = _arr[x, y];
                    var location = new Point(x, y);

                    var found = false;
                    foreach (var (point, color) in highlightedPoints)
                    {
                        if (point != location)
                            continue;
                        
                        Console.Write(builder);
                        Console.ForegroundColor = color;
                        Console.Write(element);
                        Console.ResetColor();
                        builder.Clear();
                        found = true;
                    }
                    
                    if (!found)
                        builder.Append(_arr[x, y]);
                }

                builder.AppendLine();
            }
            
            if (builder.Length > 0)
                Console.WriteLine(builder.ToString());
            
            Console.SetCursorPosition(0, 0);
        }
        
        /*
        foreach (var (point, color) in highlightedPoints)
        {
            Console.ForegroundColor = color;
            Console.SetCursorPosition(point.Y, point.X);
            Console.Write(_arr[point.X, point.Y]);
            Console.ResetColor();
        }
        
        Console.SetCursorPosition(0, _arr.GetLength(0) + 1);
        */

        //Console.ReadLine();
    }
    
    // does not un-color already colored characters
    public void PrintFast(params (Point Point, ConsoleColor Color)[] highlightedPoints)
    {

        foreach (var (point, color) in highlightedPoints)
        {
            Console.SetCursorPosition(point.Y, point.X);
            Console.ForegroundColor = color;
            Console.Write(_arr[point.X, point.Y]);
            Console.ResetColor();
        }
       
        Console.SetCursorPosition(0, _arr.GetLength(0) + 1);

        //Console.ReadLine();
    }

    public IEnumerator<(Point Point, T Value)> GetEnumerator() => ToEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _arr.GetEnumerator();

    private IEnumerable<(Point Point, T Value)> ToEnumerable()
    {
        for (var x = 0; x < _arr.GetLength(0); x++)
        {
            for (var y = 0; y < _arr.GetLength(1); y++)
            {
                var element = _arr[x, y];
                yield return (new Point(x, y), element);
            }
        }

        /*
        foreach (var item in _arr)
            yield return item;
        */
    }

    public static implicit operator T[,](Graph<T> graph) => graph._arr;

    public static implicit operator Graph<T>(T[,] arr) => new(arr);
}