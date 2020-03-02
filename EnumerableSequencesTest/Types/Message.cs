using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace EnumerableSequencesTest
{
    public class Message
    {
        private static int count = 0;
        private string message;
        private int id;

        public int Id
        {
            get { return this.id; }
        }

        /// <summary>Gets the message.</summary>
        /// <returns>Message.</returns>
        public string GetMessage()
        {
            return this.message;
        }

        /// <summary>Initializes a new instance of the <see cref="Message"/> class.</summary>
        /// <param name="message">The message.</param>
        public Message(string message)
        {
            this.message = message;
            count++;
            this.id = count;
        }
    }
}
