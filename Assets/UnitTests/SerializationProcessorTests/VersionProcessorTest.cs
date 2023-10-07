using System;
using System.Collections.Generic;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Processors;
using NUnit.Framework;

namespace Tests
{
    public class VersionProcessorTest
    {
        [Test]
        public void CanSerializeTests()
        {
            HashSet<Type> supportedTypes = new HashSet<Type>();
            TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
            VersionProcessor processor = new VersionProcessor(sd);

            Version version12 = new Version(1, 2);
            Version version123 = new Version(1, 2, 3);
            Version version1234 = new Version(1, 2, 3, 4);
            string version12Str = version12.ToString();
            string version123Str = version123.ToString();
            string version1234Str = version1234.ToString();

            List<object> allTestData = new List<object>() { null, string.Empty, " ", 101, version12, version123, version1234, version12Str, version123Str, version1234Str };
            List<object> onlyVersionData = new List<object>() { version12, version123, version1234 };
            List<object> allButVersionData = new List<object>() { null, string.Empty, " ", 101, version12Str, version123Str, version1234Str };

            GenericSerializationProcessorTester<VersionProcessor>.CanSerializeTest(processor, supportedTypes, new List<CanSerializeTestData>()
            {
                new CanSerializeTestData()
                {
                    failingValues = allTestData
                },
                new CanSerializeTestData()
                {
                    supportedTypes = new List<Type>() { typeof(Version) },
                    passingValues = onlyVersionData,
                    failingValues = allButVersionData
                },
                new CanSerializeTestData()
                {
                    supportedTypes = new List<Type>() { typeof(string) },
                    passingValues = onlyVersionData,
                    failingValues = allButVersionData
                },
                new CanSerializeTestData()
                {
                    supportedTypes = new List<Type>() { typeof(Version), typeof(string) },
                    passingValues = onlyVersionData,
                    failingValues = allButVersionData
                }
            });
        }

        [Test]
        public void CanDeserializeTests()
        {
            HashSet<Type> supportedTypes = new HashSet<Type>();
            TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
            VersionProcessor processor = new VersionProcessor(sd);

            Version version12 = new Version(1, 2);
            Version version123 = new Version(1, 2, 3);
            Version version1234 = new Version(1, 2, 3, 4);
            string version12Str = version12.ToString();
            string version123Str = version123.ToString();
            string version1234Str = version1234.ToString();

            GenericDeserializationProcessorTester<VersionProcessor>.CanDeserializeTest(processor, new List<CanDeserializeTestData>()
            {
                new CanDeserializeTestData()
                {
                    throwingValues = new List<DeserializationThrowingValue>()
                    {
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, null),
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, version12),
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, version12Str),
                    },
                    passingValues = new List<CanDeserializeValue>()
                    {
                        new CanDeserializeValue(typeof(Version), string.Empty),
                        new CanDeserializeValue(typeof(Version), " "),
                        new CanDeserializeValue(typeof(Version), "ABCDE"),
                        new CanDeserializeValue(typeof(Version), version12),
                        new CanDeserializeValue(typeof(Version), version123),
                        new CanDeserializeValue(typeof(Version), version1234),
                        new CanDeserializeValue(typeof(Version), version12Str),
                        new CanDeserializeValue(typeof(Version), version123Str),
                        new CanDeserializeValue(typeof(Version), version1234Str)
                    },
                    failingValues = new List<CanDeserializeValue>()
                    {
                        new CanDeserializeValue(typeof(string), version12),
                        new CanDeserializeValue(typeof(string), version12Str),
                        new CanDeserializeValue(typeof(Version), null),
                        new CanDeserializeValue(typeof(Version), 101),
                    }
                }
            });
        }

        [Test]
        public void SerializeTests()
        {
            HashSet<Type> supportedTypes = new HashSet<Type>();
            TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
            VersionProcessor processor = new VersionProcessor(sd);

            Version version12 = new Version(1, 2);
            Version version123 = new Version(1, 2, 3);
            Version version1234 = new Version(1, 2, 3, 4);
            string version12Str = version12.ToString();
            string version123Str = version123.ToString();
            string version1234Str = version1234.ToString();

            GenericSerializationProcessorTester<VersionProcessor>.SerializeTest(processor, supportedTypes, new List<SerializeTestData>()
            {
                new SerializeTestData()
                {
                    throwingValues = new List<SerializationThrowingValue>()
                    {
                        new SerializationThrowingValue(typeof(SerializationException), null),
                        new SerializationThrowingValue(typeof(SerializationException), string.Empty),
                        new SerializationThrowingValue(typeof(SerializationException), " "),
                        new SerializationThrowingValue(typeof(SerializationException), 101),
                        new SerializationThrowingValue(typeof(SerializationException), version12),
                        new SerializationThrowingValue(typeof(SerializationException), version123),
                        new SerializationThrowingValue(typeof(SerializationException), version1234),
                        new SerializationThrowingValue(typeof(SerializationException), version12Str),
                        new SerializationThrowingValue(typeof(SerializationException), version123Str),
                        new SerializationThrowingValue(typeof(SerializationException), version1234Str),
                    }
                },
                new SerializeTestData()
                {
                    supportedTypes = new List<Type>() { typeof(Version) },
                    passingValues = new List<SerializeValue>()
                    {
                        new SerializeValue(typeof(Version), version12),
                        new SerializeValue(typeof(Version), version123),
                        new SerializeValue(typeof(Version), version1234)
                    },
                    throwingValues = new List<SerializationThrowingValue>
                    {
                        new SerializationThrowingValue(typeof(SerializationException), null),
                        new SerializationThrowingValue(typeof(SerializationException), string.Empty),
                        new SerializationThrowingValue(typeof(SerializationException), " "),
                        new SerializationThrowingValue(typeof(SerializationException), 101),
                        new SerializationThrowingValue(typeof(SerializationException), version12Str),
                        new SerializationThrowingValue(typeof(SerializationException), version123Str),
                        new SerializationThrowingValue(typeof(SerializationException), version1234Str),
                    }
                },
                new SerializeTestData()
                {
                    supportedTypes = new List<Type>() { typeof(string) },
                    passingValues = new List<SerializeValue>()
                    {
                        new SerializeValue(typeof(string), version12),
                        new SerializeValue(typeof(string), version123),
                        new SerializeValue(typeof(string), version1234)
                    },
                    throwingValues = new List<SerializationThrowingValue>
                    {
                        new SerializationThrowingValue(typeof(SerializationException), null),
                        new SerializationThrowingValue(typeof(SerializationException), string.Empty),
                        new SerializationThrowingValue(typeof(SerializationException), " "),
                        new SerializationThrowingValue(typeof(SerializationException), 101),
                        new SerializationThrowingValue(typeof(SerializationException), version12Str),
                        new SerializationThrowingValue(typeof(SerializationException), version123Str),
                        new SerializationThrowingValue(typeof(SerializationException), version1234Str),
                    }
                },
                new SerializeTestData()
                {
                    supportedTypes = new List<Type>() { typeof(Version) },
                    passingValues = new List<SerializeValue>()
                    {
                        new SerializeValue(typeof(Version), version12),
                        new SerializeValue(typeof(Version), version123),
                        new SerializeValue(typeof(Version), version1234)
                    },
                    throwingValues = new List<SerializationThrowingValue>
                    {
                        new SerializationThrowingValue(typeof(SerializationException), null),
                        new SerializationThrowingValue(typeof(SerializationException), string.Empty),
                        new SerializationThrowingValue(typeof(SerializationException), " "),
                        new SerializationThrowingValue(typeof(SerializationException), 101),
                        new SerializationThrowingValue(typeof(SerializationException), version12Str),
                        new SerializationThrowingValue(typeof(SerializationException), version123Str),
                        new SerializationThrowingValue(typeof(SerializationException), version1234Str),
                    }
                },
            });
        }

        [Test]
        public void DeserializeTests()
        {
            HashSet<Type> supportedTypes = new HashSet<Type>();
            TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
            VersionProcessor processor = new VersionProcessor(sd);

            Version version12 = new Version(1, 2);
            Version version123 = new Version(1, 2, 3);
            Version version1234 = new Version(1, 2, 3, 4);
            string version12Str = version12.ToString();
            string version123Str = version123.ToString();
            string version1234Str = version1234.ToString();

            GenericDeserializationProcessorTester<VersionProcessor>.DeserializeTests(processor, new List<DeserializeTestData>()
            {
                new DeserializeTestData()
                {
                    throwingValues = new List<DeserializationThrowingValue>()
                    {
                        new DeserializationThrowingValue(typeof(ArgumentNullException), null, null),
                        new DeserializationThrowingValue(typeof(SerializationException), typeof(Version), null),
                        new DeserializationThrowingValue(typeof(SerializationException), typeof(string), null),
                        new DeserializationThrowingValue(typeof(SerializationException), typeof(Version), 101),
                        new DeserializationThrowingValue(typeof(SerializationException), typeof(Version), string.Empty),
                        new DeserializationThrowingValue(typeof(SerializationException), typeof(Version), " "),
                        new DeserializationThrowingValue(typeof(SerializationException), typeof(Version), "ABCDE"),
                        new DeserializationThrowingValue(typeof(SerializationException), typeof(Version), "AB.CD"),
                        new DeserializationThrowingValue(typeof(SerializationException), typeof(Version), "1.2.3.4.5"),
                        new DeserializationThrowingValue(typeof(SerializationException), typeof(Version), "1.2.-3.4."),
                    },
                    passingValues = new List<DeserializeValue>()
                    {
                        new DeserializeValue(typeof(Version), version12),
                        new DeserializeValue(typeof(Version), version123),
                        new DeserializeValue(typeof(Version), version1234),
                        new DeserializeValue(typeof(Version), version12Str),
                        new DeserializeValue(typeof(Version), version123Str),
                        new DeserializeValue(typeof(Version), version1234Str),
                    }
                }
            });
        }
    }
}