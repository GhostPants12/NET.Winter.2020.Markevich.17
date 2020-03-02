using System;
using System.Collections.Generic;
using System.Text;
using GenericsDemo;
using NUnit.Framework;

namespace EnumerableSequencesTest
{
    class CountTest
    {
        string[] stringArray = new string[5];
        int[] intArray = new int[5];
        [SetUp]
        public void Setup()
        {
            for (int i = 0; i < 5; i++)
            {
                intArray[i] = i;
                stringArray[i] = i.ToString();
            }
        }

        [Test]
        public void CountTest_IntString()
        {
            Assert.AreEqual(intArray.Count(), 5);
            Assert.AreEqual(intArray.Count((i => i%2 == 0)), 3);
            Assert.AreEqual(stringArray.Count(), 5);
            Assert.AreEqual(stringArray.Count((i => i.Length == 2)), 0);
        }
    }
}
