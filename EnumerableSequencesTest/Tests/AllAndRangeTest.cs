using System;
using System.Collections.Generic;
using System.Text;
using GenericsDemo;
using NUnit.Framework;

namespace EnumerableSequencesTest
{
    class AllAndRangeTest
    {
        List<int> intList = new List<int>();
        List<string> stringList = new List<string>();
        List<PointStruct> pointList = new List<PointStruct>();

        [SetUp]
        public void Setup()
        {
            for (int i = 0; i < 10; i++)
            {
                intList.Add(i);
                stringList.Add(i.ToString());
                pointList.Add(new PointStruct(i, i));
            }
        }

        [Test]
        public void AllTest_IntStringPoint()
        {
            Assert.IsFalse(intList.All((i => i <= 0)));
            Assert.IsTrue(stringList.All((s => s.Length == 1)));
            Assert.IsFalse(pointList.All(((p => p.x > p.y))));
        }

        [Test]
        public void RangeTest()
        {
            CollectionAssert.AreEqual(GenericsDemo.Enumerable.Range(0, 5), new int[] { 0, 1, 2, 3, 4, 5});
        }
    }
}
