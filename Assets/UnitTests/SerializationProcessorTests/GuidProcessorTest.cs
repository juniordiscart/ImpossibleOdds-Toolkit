using System;
using System.Collections.Generic;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Processors;
using NUnit.Framework;

namespace Tests
{
    public class GuidProcessorTest
    {
        [Test]
        public void CanSerializeTests()
        {
            HashSet<Type> supportedTypes = new HashSet<Type>();
            TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
            GuidProcessor processor = new GuidProcessor(sd);

            Guid guid = Guid.NewGuid();
            string guidStr = guid.ToString();

            List<object> allTestData = new List<object>() { null, string.Empty, " ", 101, guidStr, guid };
            List<object> onlyGuidTestData = new List<object>() { guid };
            List<object> allButGuidTestData = new List<object>() { null, string.Empty, " ", 101, guidStr };

            GenericSerializationProcessorTester<GuidProcessor>.CanSerializeTest(processor, supportedTypes, new List<CanSerializeTestData>()
            {
                new CanSerializeTestData
                {
                    failingValues = allTestData
                },
                new CanSerializeTestData
                {
                    supportedTypes = new List<Type>() { typeof(Guid) },
                    passingValues = onlyGuidTestData,
                    failingValues = allButGuidTestData
                },
                new CanSerializeTestData
                {
                    supportedTypes = new List<Type>() { typeof(string) },
                    passingValues = onlyGuidTestData,
                    failingValues = allButGuidTestData
                },
                new CanSerializeTestData
                {
                    supportedTypes = new List<Type>() { typeof(string), typeof(Guid) },
                    passingValues = onlyGuidTestData,
                    failingValues = allButGuidTestData
                }
            });
        }

        [Test]
        public void CanDeserializeTests()
        {
            HashSet<Type> supportedTypes = new HashSet<Type>();
            TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
            GuidProcessor processor = new GuidProcessor(sd);

            Guid guid = Guid.NewGuid();
            string guidStr = guid.ToString();

            GenericDeserializationProcessorTester<GuidProcessor>.CanDeserializeTest(processor, new List<CanDeserializeTestData>()
            {
                new CanDeserializeTestData()
                {
                    throwingValues = new List<DeserializationThrowingValue>()
                    {
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, null),
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, guid),
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, guidStr),
                    },
                    passingValues = new List<CanDeserializeValue>()
                    {
                        new CanDeserializeValue(typeof(Guid), string.Empty),
                        new CanDeserializeValue(typeof(Guid), " "),
                        new CanDeserializeValue(typeof(Guid), Guid.NewGuid().ToString()),
                        new CanDeserializeValue(typeof(Guid), Guid.NewGuid()),
                    },
                    failingValues = new List<CanDeserializeValue>()
                    {
                        new CanDeserializeValue(typeof(string), Guid.NewGuid()),
                        new CanDeserializeValue(typeof(Guid), null),
                        new CanDeserializeValue(typeof(Guid), 101),
                    }
                }
            });
        }

        [Test]
        public void SerializeTests()
        {
            HashSet<Type> supportedTypes = new HashSet<Type>();
            TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
            GuidProcessor processor = new GuidProcessor(sd);

            Guid guid = Guid.NewGuid();
            string guidStr = guid.ToString();

            GenericSerializationProcessorTester<GuidProcessor>.SerializeTest(processor, supportedTypes, new List<SerializeTestData>()
            {
                new SerializeTestData()
                {
                    throwingValues = new List<SerializationThrowingValue>()
                    {
                        new SerializationThrowingValue(typeof(SerializationException), null),
                        new SerializationThrowingValue(typeof(SerializationException), string.Empty),
                        new SerializationThrowingValue(typeof(SerializationException), " "),
                        new SerializationThrowingValue(typeof(SerializationException), 101),
                        new SerializationThrowingValue(typeof(SerializationException), guid),
                        new SerializationThrowingValue(typeof(SerializationException), guidStr),
                    }
                },
                new SerializeTestData()
                {
                    supportedTypes = new List<Type>() { typeof(Guid) },
                    passingValues = new List<SerializeValue>()
                    {
                        new SerializeValue(typeof(Guid), guid)
                    },
                    throwingValues = new List<SerializationThrowingValue>()
                    {
                        new SerializationThrowingValue(typeof(SerializationException), null),
                        new SerializationThrowingValue(typeof(SerializationException), string.Empty),
                        new SerializationThrowingValue(typeof(SerializationException), " "),
                        new SerializationThrowingValue(typeof(SerializationException), 101),
                        new SerializationThrowingValue(typeof(SerializationException), guidStr),
                    }
                },
                new SerializeTestData()
                {
                    supportedTypes = new List<Type>() { typeof(string) },
                    passingValues = new List<SerializeValue>()
                    {
                        new SerializeValue(typeof(string), guid)
                    },
                    throwingValues = new List<SerializationThrowingValue>()
                    {
                        new SerializationThrowingValue(typeof(SerializationException), null),
                        new SerializationThrowingValue(typeof(SerializationException), string.Empty),
                        new SerializationThrowingValue(typeof(SerializationException), " "),
                        new SerializationThrowingValue(typeof(SerializationException), 101),
                        new SerializationThrowingValue(typeof(SerializationException), guidStr),
                    }
                },
                // Serialization definition supports both Guid and string types.
                new SerializeTestData()
                {
                    supportedTypes = new List<Type>() { typeof(string), typeof(Guid) },
                    passingValues = new List<SerializeValue>()
                    {
                        new SerializeValue(typeof(Guid), guid)
                    },
                    throwingValues = new List<SerializationThrowingValue>()
                    {
                        new SerializationThrowingValue(typeof(SerializationException), null),
                        new SerializationThrowingValue(typeof(SerializationException), string.Empty),
                        new SerializationThrowingValue(typeof(SerializationException), " "),
                        new SerializationThrowingValue(typeof(SerializationException), 101),
                        new SerializationThrowingValue(typeof(SerializationException), guidStr),
                    }
                }
            });
        }

        [Test]
        public void DeserializeTests()
        {
            HashSet<Type> supportedTypes = new HashSet<Type>();
            TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
            GuidProcessor processor = new GuidProcessor(sd);

            GenericDeserializationProcessorTester<GuidProcessor>.DeserializeTests(processor, new List<DeserializeTestData>()
            {
                new DeserializeTestData()
                {
                    throwingValues = new List<DeserializationThrowingValue>()
                    {
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, null),
                        new DeserializationThrowingValue(typeof(SerializationException), typeof(Guid), null),
                        new DeserializationThrowingValue(typeof(SerializationException), typeof(string), null),
                        new DeserializationThrowingValue(typeof(SerializationException), typeof(Guid), 101),
                        new DeserializationThrowingValue(typeof(SerializationException), typeof(Guid), string.Empty),
                        new DeserializationThrowingValue(typeof(SerializationException), typeof(Guid), DateTime.Now.ToString()),
                        new DeserializationThrowingValue(typeof(SerializationException), typeof(Guid), "ABCDE"),
                    },
                    passingValues = new List<DeserializeValue>()
                    {
                        new DeserializeValue(typeof(Guid), Guid.Empty),
                        new DeserializeValue(typeof(Guid), Guid.Empty.ToString()),
                        new DeserializeValue(typeof(Guid), Guid.NewGuid()),
                        new DeserializeValue(typeof(Guid), Guid.NewGuid().ToString()),
                    }
                }
            });
        }
    }
}