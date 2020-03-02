using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace EnumerableSequencesTest
{

    public struct PointStruct
    {
        public int x;
        public int y;

        /// <summary>Initializes a new instance of the <see cref="PointStruct"/> struct.</summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public PointStruct(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
