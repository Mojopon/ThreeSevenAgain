using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

public static class IEnumerableExtension
{
    public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first,
        IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
    {
        Assert.IsNotNull(first);
        Assert.IsNotNull(second);
        Assert.IsNotNull(resultSelector);

        using (var firstEnumerator = first.GetEnumerator())
        using (var secondEnumerator = second.GetEnumerator())
        {
            while (firstEnumerator.MoveNext() && secondEnumerator.MoveNext())
            {
                yield return resultSelector(firstEnumerator.Current, secondEnumerator.Current);
            }
        }
    }

    public static IEnumerable<IEnumerable<T>> Buffer<T>(this IEnumerable<T> source, int count)
    {
        if (source == null) throw new ArgumentNullException("source");
        return BufferImplements(source, count);
    }
    private static IEnumerable<IEnumerable<T>> BufferImplements<T>(IEnumerable<T> source, int count)
    {
        var result = new List<T>(count);
        foreach (var item in source)
        {
            result.Add(item);
            if (result.Count == count)
            {
                yield return result;
                result = new List<T>(count);
            }
        }
        if (result.Count != 0)
            yield return result;
    }
}
