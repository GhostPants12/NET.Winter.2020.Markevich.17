using System;
using System.Collections.Generic;
using System.Text;

namespace GenericsDemo
{
    public interface IComparer<in T>
    {
        /// <summary>Compares two values.</summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>Int number.</returns>
        int Compare(T lhs, T rhs);
    }
}
