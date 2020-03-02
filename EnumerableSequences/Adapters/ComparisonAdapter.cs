using System;
using System.Collections.Generic;
using System.Text;

namespace Enumerable.Adapters
{
    internal class ComparisonAdapter<TSource> : GenericsDemo.IComparer<TSource>
    {
        private Comparison<TSource> comparison;

        /// <summary>Initializes a new instance of the <see cref="ComparisonAdapter{TSource}"/> class.</summary>
        /// <param name="comparison">The comparison.</param>
        public ComparisonAdapter(Comparison<TSource> comparison)
        {
            this.comparison = comparison;
        }

        /// <summary>Compares two values.</summary>
        /// <param name="lhs">The left value.</param>
        /// <param name="rhs">The right value.</param>
        /// <returns> Result of comparison.</returns>
        public int Compare(TSource lhs, TSource rhs)
        {
            return this.comparison(lhs, rhs);
        }
    }
}
