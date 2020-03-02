using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using Enumerable.Adapters;
using GenericsDemo;

namespace GenericsDemo
{
    public static class Enumerable
    {
        /// <summary>Converts to array.</summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>Array of TSource.</returns>
        public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
        {
            CheckParams(source);
            int length = source.Count();
            int i = 0;
            TSource[] result = new TSource[length];
            foreach (var element in source)
            {
                result[i] = element;
                i++;
            }

            return result;
        }

        /// <summary>Filters source by specified predicate.</summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>Sorted enumerable sequence.</returns>
        /// <exception cref="ArgumentNullException">source is null or predicate is null.</exception>
        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            CheckParams(source, predicate);

            return WhereIterator(source, predicate);

            IEnumerable<TSource> WhereIterator(IEnumerable<TSource> sourceEnumerable, Func<TSource, bool> pred)
            {
                foreach (var element in source)
                {
                    if (predicate(element) == true)
                    {
                        yield return element;
                    }
                }
            }
        }

        /// <summary>Transforms the enumerable sequence to the sequence of another type by the specified transformer.</summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="transformer">The transformer.</param>
        /// <returns>Sequence of TResult type.</returns>
        /// <exception cref="ArgumentNullException">source is null or transformer is null.</exception>
        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> transformer)
        {
            CheckParams(source, transformer);

            return SelectIterator(source, transformer);

            IEnumerable<TResult> SelectIterator(IEnumerable<TSource> sourceEnumerable,
                Func<TSource, TResult> transformer)
            {
                foreach (var element in source)
                {
                    yield return transformer(element);
                }
            }
        }

        /// <summary>Orders the array according to some rule.</summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="comparer">The comparing rule.</param>
        /// <returns>Transformed array.</returns>
        /// <exception cref="ArgumentNullException">Source array is null or comparing rule is null.</exception>
        public static IEnumerable<TSource> OrderAccordingTo<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer)
        {
            CheckParams(source, comparer);
            TSource element;

            var returnArray = new TSource[source.Count()];
            int i = 0;

            foreach (var sourceElement in source)
            {
                returnArray[i] = sourceElement;
                i++;
            }

            for (i = 0; i < returnArray.Length; i++)
            {
                for (int j = i; j < returnArray.Length; j++)
                {
                    if (comparer.Compare(returnArray[i], returnArray[j]) < 0)
                    {
                        element = returnArray[i];
                        returnArray[i] = returnArray[j];
                        returnArray[j] = element;
                    }
                }
            }

            foreach (var sourceElement in returnArray)
            {
                yield return sourceElement;
            }
        }

        /// <summary>Orders the according to specified comparing rules.</summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>Sorted enumerable sequence.</returns>
        /// <exception cref="ArgumentNullException">source is null or comparer is null.</exception>
        public static IEnumerable<TSource> OrderAccordingTo<TSource>(this IEnumerable<TSource> source, Comparison<TSource> comparer)
        {
            CheckParams(source, comparer);
            return OrderAccordingTo(source, new ComparisonAdapter<TSource>(comparer));
        }

        /// <summary>Orders the enumerable sequence by specified predicate.</summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>Sorted enumerable.</returns>
        public static IEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> predicate)
        {
            SortedDictionary<TKey, TSource> dic = new SortedDictionary<TKey, TSource>(Comparer<TKey>.Default);
            foreach (var item in source)
            {
                dic.Add(predicate(item), item);
            }

            foreach (var item in dic)
            {
                yield return item.Value;
            }
        }

        /// <summary>Orders the enumerable sequence by specified predicate.</summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>Sorted enumerable.</returns>
        public static IEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> predicate, System.Collections.Generic.IComparer<TKey> comparer)
        {
            SortedDictionary<TKey, TSource> dic = new SortedDictionary<TKey, TSource>(comparer);
            foreach (var item in source)
            {
                dic.Add(predicate(item), item);
            }

            foreach (var item in dic)
            {
                yield return item.Value;
            }
        }

        /// <summary>Orders the enumerable by specified predicate in a descending order.</summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>Sorted enumerable.</returns>
        public static IEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> predicate)
        {
            return OrderBy(source, predicate).Reverse();
        }

        /// <summary>Orders the enumerable by specified predicate in a descending order.</summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>Sorted enumerable.</returns>
        public static IEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> predicate, System.Collections.Generic.IComparer<TKey> comparer)
        {
            return OrderBy(source, predicate, comparer).Reverse();
        }

        /// <summary>  Returns source array's elements of the specified type.</summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>Returns the array of specified type.</returns>
        /// <exception cref="ArgumentNullException">Source is null.</exception>
        public static IEnumerable<TSource> OfType<TSource>(this IEnumerable source)
        {
            CheckParams(source);
            foreach (var element in source)
            {
                if (element is TSource)
                {
                    yield return (TSource)element;
                }
            }
        }

        /// <summary>  Casts source array's elements of the specified type.</summary>
        /// <typeparam name="T">The type of the return array.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>Returns the array of specified type.</returns>
        /// <exception cref="ArgumentNullException">Source is null.</exception>
        public static IEnumerable<T> Cast<T>(this IEnumerable source)
        {
            CheckParams(source);
            if (source is IEnumerable<T> returnSource)
            {
                return returnSource;
            }

            return CastIterator(source);
            IEnumerable<T> CastIterator(IEnumerable arr)
            {
                foreach (object obj in arr)
                {
                    yield return (T)obj!;
                }
            }
        }

        /// <summary>  Checks if all elements of the array are matching the specified predicate.</summary>
        /// <typeparam name="T">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>True - if matches, false - if not.</returns>
        public static bool All<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            foreach (var item in source)
            {
                if (!predicate(item))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Reverses the specified source.</summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>Reversed array.</returns>
        /// <exception cref="ArgumentNullException">Source array is null.</exception>
        public static IEnumerable<TSource> Reverse<TSource>(this IEnumerable<TSource> source)
        {
            CheckParams(source);

            TSource[] returnArray = new TSource[source.Count()];
            int i = source.Count() - 1;
            foreach (var element in source)
            {
                returnArray[i] = element;
                i--;
            }

            foreach (var element in returnArray)
            {
                yield return element;
            }
        }

        /// <summary>Generates the sequence of integer numbers within a specified range.</summary>
        /// <param name="start">Start value.</param>
        /// <param name="count">The number of elements.</param>
        /// <returns>Sequence of integer numbers.</returns>
        public static IEnumerable<int> Range(int start, int count)
        {
            for (int i = start; i <= start + count; i++)
            {
                yield return i;
            }
        }

        /// <summary>Counts how many elements are there in enumerable sequence.</summary>
        /// <typeparam name="T">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>Number of elements.</returns>
        public static int Count<T>(this IEnumerable<T> source)
        {
            int counter = 0;
            foreach (var item in source)
            {
                counter++;
            }

            return counter;
        }

        /// <summary>Counts how many elements are there in enumerable sequence that match specified predicate.</summary>
        /// <typeparam name="T">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>Number of elements.</returns>
        public static int Count<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            int counter = 0;
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    counter++;
                }
            }

            return counter;
        }

        /// <summary>Checks the parameters.</summary>
        /// <param name="source">The source.</param>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        private static void CheckParams(object source)
        {
            source = source ?? throw new ArgumentNullException($"{nameof(source)} cannot be null.");
        }

        /// <summary>Checks the parameters.</summary>
        /// <param name="source">The source.</param>
        /// <exception cref="ArgumentNullException">source is null or param is null.</exception>
        private static void CheckParams(object source, object param)
        {
            CheckParams(source);
            param = param ?? throw new ArgumentNullException($"{nameof(param)} cannot be null.");
        }
    }
}
