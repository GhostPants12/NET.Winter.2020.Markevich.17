using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GenericsDemo;
using NUnit.Framework;

namespace EnumerableSequencesTest
{
    public class OfTypeTests
    {
        Stack stack = new Stack();

        [SetUp]
        public void Setup()
        {
            stack.Push(1);
            stack.Push("1");
            stack.Push(new PointStruct(1,1));
            stack.Push(2);
            stack.Push("2");
            stack.Push(new PointStruct(2, 2));
        }

        [Test]
        public void OfTypeTest_IntStringPoint()
        {
            CollectionAssert.AreEqual(stack.OfType<int>(), new int[] {2,1});
            CollectionAssert.AreEqual(stack.OfType<string>(), new string[] { "2", "1" });
            CollectionAssert.AreEqual(stack.OfType<PointStruct>(), new PointStruct[] { new PointStruct(2,2), new PointStruct(1,1) });
        }
    }
}
