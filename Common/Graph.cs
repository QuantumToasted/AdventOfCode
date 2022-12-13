using System.Collections;

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

    public int TotalLength => _arr.Length;

    public int GetLength(GraphDimension dimension)
        => _arr.GetLength((int) dimension);

    public IEnumerator<T> GetEnumerator() => ToEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _arr.GetEnumerator();

    private IEnumerable<T> ToEnumerable()
    {
        foreach (var item in _arr)
            yield return item;
    }
}