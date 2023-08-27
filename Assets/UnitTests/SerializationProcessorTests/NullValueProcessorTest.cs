using System;
using System.Collections.Generic;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Processors;
using NUnit.Framework;

namespace Tests
{
    public class NullValueProcessorTest
    {
        [Test]
        public void CanDeserializeTests()
        {
            HashSet<Type> supportedTypes = new HashSet<Type>();
            TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
            NullValueProcessor processor = new NullValueProcessor(sd);

            GenericDeserializationProcessorTester<NullValueProcessor>.CanDeserializeTest(processor, new List<CanDeserializeTestData>()
            {
                new CanDeserializeTestData()
                {
                    throwingValues = new List<DeserializationThrowingValue>()
                    {
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, null),
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, 123),
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, "123"),
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, string.Empty)
                    },
                    failingValues = new List<CanDeserializeValue>()
                    {
                        new CanDeserializeValue(typeof(int), 123),
                        new CanDeserializeValue(typeof(int), "123"),
                        new CanDeserializeValue(typeof(float), 123f),
                        new CanDeserializeValue(typeof(string), "123"),
                        new CanDeserializeValue(typeof(string), string.Empty),
                    },
                    passingValues = new List<CanDeserializeValue>()
                    {
                        new CanDeserializeValue(typeof(int), null),
                        new CanDeserializeValue(typeof(float), null),
                        new CanDeserializeValue(typeof(string), null),
                    }
                }
            });
        }

        [Test]
        public void DeserializeTests()
        {
            HashSet<Type> supportedTypes = new HashSet<Type>();
            TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
            NullValueProcessor processor = new NullValueProcessor(sd);

            GenericDeserializationProcessorTester<NullValueProcessor>.DeserializeTests(processor, new List<DeserializeTestData>()
            {
                new DeserializeTestData()
                {
                    throwingValues = new List<DeserializationThrowingValue>()
                    {
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, null),
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, 123),
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, 123f),
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, "123"),
                        new DeserializationThrowingValue(typeof(SerializationException), typeof(int), 123),
                        new DeserializationThrowingValue(typeof(SerializationException), typeof(float), 123f),
                        new DeserializationThrowingValue(typeof(SerializationException), typeof(string), "123"),
                        new DeserializationThrowingValue(typeof(SerializationException), typeof(string), string.Empty),
                    },
                    passingValues = new List<DeserializeValue>()
                    {
                        new DeserializeValue(typeof(int), null),
                        new DeserializeValue(typeof(float), null),
                        new DeserializeValue(typeof(string), null)
                    }
                }
            });
        }
    }
}