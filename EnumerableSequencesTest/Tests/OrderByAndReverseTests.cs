using System;
using System.Collections.Generic;
using System.Text;
using GenericsDemo;
using NUnit.Framework;

namespace EnumerableSequencesTest
{
    public class OrderByAndReverseTests
    {
        readonly List<int> intList = new List<int>();
        readonly List<int> reverseIntList = new List<int>();
        readonly  List<string> stringList = new List<string>();
        readonly List<string> reverseStringList = new List<string>();
        readonly List<Message> messageList = new List<Message>();

        [SetUp]
        public void Setup()
        {
            for (int i = 0; i < 10; i++)
            {
                intList.Add(i);
                reverseIntList.Add(i);
                messageList.Add(new Message(i.ToString()));
            }
        }

        [Test]
        public void OrderByAndReverseTest_ForIntStringMessage()
        {
            int a = 10;
            reverseIntList.Reverse();
            reverseStringList.Reverse();
            CollectionAssert.AreEqual(intList.OrderBy(new Func<int, int>((i => 0 - i))), reverseIntList);
            CollectionAssert.AreEqual(stringList.OrderBy(s => a--), reverseStringList);
            CollectionAssert.AreEqual(messageList.OrderByDescending(message => message.Id, new IntComparer()), messageList);
        }
    }
}
