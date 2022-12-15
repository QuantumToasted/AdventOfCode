using System.Collections;
using System.Drawing;
using System.Text;

namespace AdventOfCode.Common;

public enum GraphDimension
{
    X,
    Y
}

public class Graph<T> : IEnumerable<T>
    where T : struct
{
    private readonly T[,] _arr;

    public Graph(T[,] arr)
    {
        _arr = arr;
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
        var list = new List<T>();
        
        if (y > 0)
        {
            var value = _arr[x, y - 1];
            if (predicate?.Invoke(value) != false)
                list.Add(value);
        }

        if (y < _arr.GetLength(1) - 1)
        {
            var value = _arr[x, y + 1];
            if (predicate?.Invoke(value) != false)
                list.Add(value);
        }

        if (x > 0)
        {
            var value = _arr[x - 1, y];
            if (predicate?.Invoke(value) != false)
                list.Add(value);
        }

        if (x < _arr.GetLength(0) - 1)
        {
            var value = _arr[x + 1, y];
            if (predicate?.Invoke(value) != false)
                list.Add(value);
        }

        return list.ToArray();
    }

    public int TotalLength => _arr.Length;

    public int GetLength(GraphDimension dimension)
        => _arr.GetLength((int) dimension);

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
        
            Console.WriteLine(builder.ToString());
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

    public IEnumerator<T> GetEnumerator() => ToEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _arr.GetEnumerator();

    private IEnumerable<T> ToEnumerable()
    {
        foreach (var item in _arr)
            yield return item;
    }

    public static implicit operator T[,](Graph<T> graph) => graph._arr;

    public static implicit operator Graph<T>(T[,] arr) => new(arr);
}