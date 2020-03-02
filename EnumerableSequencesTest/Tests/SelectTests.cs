using System;
using System.Collections.Generic;
using System.Text;
using GenericsDemo;
using Moq;
using NUnit.Framework;
using Enumerable = GenericsDemo.Enumerable;

namespace EnumerableSequencesTest
{
    public class SelectTests
    {
        readonly List<PointStruct> pointList = new List<PointStruct>();
        string[] stringArray = new string[10];

        [SetUp]
        public void Setup()
        {
            for (int i = 0; i < 10; i++)
            {
                pointList.Add(new PointStruct(i,i+1));
                stringArray[i] = i.ToString();
            }
        }

        [Test]
        public void SelectTests_IntStringPoint()
        {
            int[] localIntArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            CollectionAssert.AreEqual(localIntArray.Select((a => new PointStruct(a, a + 1))), pointList);
            CollectionAssert.AreEqual(localIntArray.Select((i => i.ToString())), stringArray);
        }
    }
}