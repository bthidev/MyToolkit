using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Toolkit.Extention
{
    public static class EnumerableExtensions
    {
        [SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "It's OK")]
        public static async Task<T[]> ToArrayAsync<T>(this Task<IEnumerable<T>> source)
        {
            var awaited = await source.ConfigureAwait(false);
            return awaited is T[] arr ? arr : awaited.ToArray();
        }

        [SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "It's OK")]
        public static async Task<IDictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this Task<IEnumerable<TSource>> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
             where TKey : notnull
        {
            var awaited = await source.ConfigureAwait(false);
            return awaited.ToDictionary(keySelector, elementSelector);
        }

        [SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "It's OK")]
        public static async Task<List<T>> ToListAsync<T>(this Task<IEnumerable<T>> source)
        {
            var awaited = await source.ConfigureAwait(false);
            return awaited is List<T> lst ? lst : awaited?.ToList();
        }

        [SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "It's OK")]
        public static async Task<T> FirstOrDefaultAsync<T>(this Task<IEnumerable<T>> source)
        {
            var awaited = await source.ConfigureAwait(false);
            return awaited.FirstOrDefault();
        }

        [SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "It's OK")]
        public static async Task<T> SingleOrDefaultAsync<T>(this Task<IEnumerable<T>> source)
        {
            var awaited = await source.ConfigureAwait(false);
            return awaited.SingleOrDefault();
        }

        [SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "It's OK")]
        public static async Task<int> CountAsync<T>(this Task<IEnumerable<T>> source)
        {
            var awaited = await source.ConfigureAwait(false);
            return awaited.Count();
        }

        public static IEnumerable<TSource> DistinctByParam<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }

        public static IEnumerable<T> Without<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source.Where(t => !predicate(t));
        }

        public static IEnumerable<T> Foreach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            return
                collection
                .Select(x =>
                {
                    action(x);
                    return x;
                })
                .ToArray();
        }
    }
}
