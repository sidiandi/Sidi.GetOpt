using System;
using System.IO;

namespace Sidi.GetOpt.Test
{
    [Usage("Test the parsing of supported value types")]
    internal class ValueParserTestApplication
    {
        public ValueParserTestApplication()
        {
        }

        [Usage("Birthday")]
        public DateTime Birthday { get; set; }

        [Usage("Duration of celebration")]
        public TimeSpan Duration { get; set; }

        [Usage("Fruit to eat")]
        public Fruits Fruit { get; set; }

        public enum Fruits
        {
            Apple,
            Orange,
            Pear,
            Melon
        };
    }
}