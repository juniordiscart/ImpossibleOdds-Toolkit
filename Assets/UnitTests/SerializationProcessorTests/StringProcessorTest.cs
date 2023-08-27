using System;
using System.Collections.Generic;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Processors;
using NUnit.Framework;

namespace Tests
{
    public class StringProcessorTest
    {
        [Test]
        public void CanDeserializeTests()
        {
            HashSet<Type> supportedTypes = new HashSet<Type>();
            TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
            StringProcessor processor = new StringProcessor(sd);

            GenericDeserializationProcessorTester<StringProcessor>.CanDeserializeTest(processor, new List<CanDeserializeTestData>()
            {
                new CanDeserializeTestData()
                {
                    throwingValues = new List<DeserializationThrowingValue>()
                    {
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, null),
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, string.Empty),
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, 123)
                    },
                    passingValues = new List<CanDeserializeValue>()
                    {
                        new CanDeserializeValue(typeof(string), string.Empty),
                        new CanDeserializeValue(typeof(string), " "),
                        new CanDeserializeValue(typeof(string), "\t\n"),
                        new CanDeserializeValue(typeof(string), "123"),
                        new CanDeserializeValue(typeof(string), "alpha"),
                        new CanDeserializeValue(typeof(string), 123),
                        new CanDeserializeValue(typeof(string), 123f),
                    },
                    failingValues = new List<CanDeserializeValue>()
                    {
                        new CanDeserializeValue(typeof(string), null),
                        new CanDeserializeValue(typeof(string), Guid.Empty),
                        new CanDeserializeValue(typeof(int), string.Empty),
                    }
                }
            });
        }

        [Test]
        public void DeserializeTests()
        {
            HashSet<Type> supportedTypes = new HashSet<Type>();
            TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
            StringProcessor processor = new StringProcessor(sd);

            GenericDeserializationProcessorTester<StringProcessor>.DeserializeTests(processor, new List<DeserializeTestData>()
            {
                new DeserializeTestData()
                {
                    throwingValues = new List<DeserializationThrowingValue>()
                    {
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, null),
                        new DeserializationThrowingValue(typeof(SerializationException), typeof(string), null),
                        new DeserializationThrowingValue(typeof(SerializationException), typeof(int), null),
                    },
                    passingValues = new List<DeserializeValue>()
                    {
                        new DeserializeValue(typeof(string), string.Empty),
                        new DeserializeValue(typeof(string), " "),
                        new DeserializeValue(typeof(string), "\t\n"),
                        new DeserializeValue(typeof(string), "alpha"),
                        new DeserializeValue(typeof(string), "123"),
                        new DeserializeValue(typeof(string), 123),
                        new DeserializeValue(typeof(string), 123f),
                    }
                }
            });
        }
    }

}