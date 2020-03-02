using System;
using System.Collections.Generic;
using GenericsDemo;
using Moq;
using NUnit.Framework;

namespace EnumerableSequencesTest
{
    public class Tests
    {
        readonly List<int> intList = new List<int>();
        readonly List<string> stringList = new List<string>();
        readonly List<Message> messageList = new List<Message>();
        readonly List<PointStruct> pointList = new List<PointStruct>();
        readonly int[] intArray = new int[10];
        readonly string[] stringArray = new string[10];
        readonly Message[] messageArray = new Message[10];
        readonly PointStruct[] pointArray = new PointStruct[10];
        [SetUp]
        public void Setup()
        {
            int intBuf;
            string stringBuf;
            Message messageBuf;
            for (int i = 0; i < 10; i++)
            {
                intBuf = i;
                stringBuf = i.ToString();
                messageBuf = new Message(stringBuf);
                intList.Add(intBuf);
                stringList.Add(stringBuf);
                messageList.Add(messageBuf);
                pointList.Add(new PointStruct(i, i+1));;
                intArray[i] = intBuf;
                stringArray[i] = stringBuf;
                messageArray[i] = messageBuf;
                pointArray[i] = new PointStruct(i, i + 1);
            }
        }

        [Test]
        public void ToArrayTests()
        {
            List<int> localIntList = new List<int>();
            localIntList.AddRange(intList);
            CollectionAssert.AreEqual(localIntList.ToArray(), intArray);
            CollectionAssert.AreEqual(stringList.ToArray(), stringArray);
            CollectionAssert.AreEqual(messageList.ToArray(), messageArray);
            CollectionAssert.AreEqual(pointList.ToArray(), pointArray);
        }

        [Test]
        public void WhereTests()
        {
            List<int> localIntList = new List<int>();
            localIntList.AddRange(intList);
            CollectionAssert.AreEqual(localIntList.Where(new Func<int, bool>((i => (i >= 0)))), intList);
            CollectionAssert.AreEqual(stringList.Where((s => s.Length > 2)), Array.Empty<string>());
            CollectionAssert.AreEqual(messageList.Where((message => message.GetMessage().Length >= 0)), messageList);
            CollectionAssert.AreEqual(pointList.Where((point => (point.y >= 11))), Array.Empty<PointStruct>());
        }
    }
}