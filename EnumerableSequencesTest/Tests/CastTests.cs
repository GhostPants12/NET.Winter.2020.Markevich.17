using System;
using System.Collections.Generic;
using System.Text;
using GenericsDemo;
using NUnit.Framework;

namespace EnumerableSequencesTest
{
    class CastTests
    {
        private IntComparer intComparer;
        readonly List<IntComparer> intList = new List<IntComparer>();
        [SetUp]
        public void Setup()
        {
            intComparer= new IntComparer();
            for(int i=0;i<5;i++)
            {
                intList.Add(intComparer);
            }
        }

        [Test]
        public void CastTest_IntComparer()
        {
           CollectionAssert.AreEqual(intList.Cast<System.Collections.Generic.IComparer<int>>(), new System.Collections.Generic.IComparer<int>[] {intComparer, intComparer, intComparer, intComparer, intComparer });
        }
    }
}
