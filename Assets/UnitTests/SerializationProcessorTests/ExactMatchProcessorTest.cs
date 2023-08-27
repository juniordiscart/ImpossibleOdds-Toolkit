using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Processors;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class ExactMatchProcessorTest
    {
        [Test]
        public void CanSerializeTests()
        {
            HashSet<Type> supportedTypes = new HashSet<Type>();
            TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
            ExactMatchProcessor processor = new ExactMatchProcessor(sd);

            GenericSerializationProcessorTester<ExactMatchProcessor>.CanSerializeTest(processor, supportedTypes, new List<CanSerializeTestData>()
            {
                new CanSerializeTestData()
                {
                    failingValues = new List<object>(){null, 123, 123f, "123"}
                },
                new CanSerializeTestData()
                {
                    supportedTypes = new List<Type>(){typeof(int)},
                    failingValues = new List<object>(){null, 123f, "123"},
                    passingValues = new List<object>(){123}
                },
                new CanSerializeTestData()
                {
                    supportedTypes = new List<Type>(){typeof(int), typeof(float), typeof(string)},
                    failingValues = new List<object>(){null},
                    passingValues = new List<object>(){123, 123f, "123"}
                }
            });
        }

        [Test]
        public void CanDeserializeTests()
        {
            HashSet<Type> supportedTypes = new HashSet<Type>();
            TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
            ExactMatchProcessor processor = new ExactMatchProcessor(sd);

            GenericDeserializationProcessorTester<ExactMatchProcessor>.CanDeserializeTest(processor, new List<CanDeserializeTestData>()
            {
                new CanDeserializeTestData()
                {
                    throwingValues = new List<DeserializationThrowingValue>()
                    {
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, null),
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, 123),
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, "123")
                    },
                    passingValues = new List<CanDeserializeValue>()
                    {
                        new CanDeserializeValue(typeof(int), 123),
                        new CanDeserializeValue(typeof(float), 123f),
                        new CanDeserializeValue(typeof(string), "123"),
                        new CanDeserializeValue(typeof(string), null),
                        new CanDeserializeValue(typeof(XElement), new XElement("Test"))
                    },
                    failingValues = new List<CanDeserializeValue>()
                    {
                        new CanDeserializeValue(typeof(int), 123f),
                        new CanDeserializeValue(typeof(int), null),
                        new CanDeserializeValue(typeof(int), "123"),
                        new CanDeserializeValue(typeof(float), 123),
                        new CanDeserializeValue(typeof(float), null),
                        new CanDeserializeValue(typeof(string), 123)
                    }
                }
            });
        }

        [Test]
        public void SerializeTests()
        {
            HashSet<Type> supportedTypes = new HashSet<Type>();
            TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
            ExactMatchProcessor processor = new ExactMatchProcessor(sd);

            GenericSerializationProcessorTester<ExactMatchProcessor>.SerializeTest(processor, supportedTypes, new List<SerializeTestData>()
            {
                new SerializeTestData()
                {
                    throwingValues = new List<SerializationThrowingValue>()
                    {
                        new SerializationThrowingValue(typeof(SerializationException), null),
                        new SerializationThrowingValue(typeof(SerializationException), 123),
                        new SerializationThrowingValue(typeof(SerializationException), 123f),
                        new SerializationThrowingValue(typeof(SerializationException), "123")
                    }
                },
                new SerializeTestData()
                {
                    supportedTypes = new List<Type>(){typeof(int)},
                    passingValues = new List<SerializeValue>()
                    {
                        new SerializeValue(typeof(int), 123)
                    },
                    throwingValues =  new List<SerializationThrowingValue>()
                    {
                        new SerializationThrowingValue(typeof(SerializationException), null),
                        new SerializationThrowingValue(typeof(SerializationException), 123f),
                        new SerializationThrowingValue(typeof(SerializationException), "123")
                    }
                },
                new SerializeTestData()
                {
                    supportedTypes = new List<Type>(){typeof(int), typeof(float), typeof(string)},
                    passingValues = new List<SerializeValue>()
                    {
                        new SerializeValue(typeof(int), 123),
                        new SerializeValue(typeof(float), 123f),
                        new SerializeValue(typeof(string), "123")
                    },
                    throwingValues = new List<SerializationThrowingValue>()
                    {
                        new SerializationThrowingValue(typeof(SerializationException), null),
                        new SerializationThrowingValue(typeof(SerializationException), Guid.NewGuid())
                    }
                }
            });
        }

        [Test]
        public void DeserializeTests()
        {
            HashSet<Type> supportedTypes = new HashSet<Type>();
            TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
            ExactMatchProcessor processor = new ExactMatchProcessor(sd);

            GenericDeserializationProcessorTester<ExactMatchProcessor>.DeserializeTests(processor, new List<DeserializeTestData>()
            {
                new DeserializeTestData()
                {
                    throwingValues = new List<DeserializationThrowingValue>()
                    {
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, null),
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, 123),
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, 123f),
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, "123"),
                        new DeserializationThrowingValue(typeof(SerializationException), typeof(int), 123f),
                        new DeserializationThrowingValue(typeof(SerializationException), typeof(int), "123"),
                    },
                    passingValues = new List<DeserializeValue>()
                    {
                        new DeserializeValue(typeof(int), 123),
                        new DeserializeValue(typeof(float), 123f),
                        new DeserializeValue(typeof(string), "123"),
                        new DeserializeValue(typeof(string), null),
                        new DeserializeValue(typeof(string), string.Empty),
                    }
                }
            });
        }
    }
}