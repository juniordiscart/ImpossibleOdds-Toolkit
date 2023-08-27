using System;
using System.Collections.Generic;
using ImpossibleOdds.Serialization.Processors;
using NUnit.Framework;

namespace Tests
{
    public class EnumProcessorTest
    {
        public enum Enum1
        {
            None,
            First,
            Second
        }

        public enum Enum2
        {

        }

        [Test]
        public void CanSerializeTest()
        {
            HashSet<Type> supportedTypes = new HashSet<Type>();
            TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
            NullValueProcessor processor = new NullValueProcessor(sd);
        }


    }
}