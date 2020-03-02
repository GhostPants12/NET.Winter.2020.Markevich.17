using System;
using System.Collections.Generic;
using System.Text;
using GenericsDemo;

namespace Enumerable.Adapters
{
    internal class PredicateAdapter<TSource> : IPredicate<TSource>
    {
        private Predicate<TSource> predicate;

        /// <summary>Initializes a new instance of the <see cref="PredicateAdapter{TSource}"/> class.</summary>
        /// <param name="predicate">The predicate.</param>
        public PredicateAdapter(Predicate<TSource> predicate)
        {
            this.predicate = predicate;
        }

        /// <summary>Determines whether an integer number matches a specific condition.</summary>
        /// <param name="value">Integer number.</param>
        /// <returns>true if an integer number matches a specific condition; otherwise, false.</returns>
        public bool IsMatch(TSource value)
        {
            return this.predicate(value);
        }
    }
}
