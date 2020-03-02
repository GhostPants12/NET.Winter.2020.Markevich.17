using System;
using System.Collections.Generic;
using System.Text;

namespace GenericsDemo
{
    public interface ITransformer<in TSource, out TResult>
    {
        /// <summary>Transforms the specified value to the another value.</summary>
        /// <param name="value">The value.</param>
        /// <returns>Transformed value.</returns>
        TResult Transform(TSource value);
    }
}
