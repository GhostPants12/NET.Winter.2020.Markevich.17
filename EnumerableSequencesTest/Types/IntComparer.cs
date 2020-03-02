using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace EnumerableSequencesTest
{
    public class IntComparer : IComparer<int>
    {
        public int Compare([AllowNull] int x, [AllowNull] int y)
        => (x < y) ? 1 : -1;
    }
}
