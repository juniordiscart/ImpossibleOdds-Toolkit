using System;
using System.Collections.Generic;
using System.Globalization;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Processors;
using NUnit.Framework;

namespace Tests
{
	public class DecimalProcessorTest
	{
		[Test]
		public void CanSerializeTests()
		{
			HashSet<Type> supportedTypes = new HashSet<Type>();
			TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
			DecimalProcessor processor = new DecimalProcessor(sd);

			decimal dec = 123.12345M;
			string decStr = dec.ToString(CultureInfo.InvariantCulture);

			GenericSerializationProcessorTester<DecimalProcessor>.CanSerializeTest(processor, supportedTypes, new List<CanSerializeTestData>()
			{
				new CanSerializeTestData()
				{
					failingValues = new List<object>() { null, string.Empty, " ", 101, decStr, dec }
				},
				new CanSerializeTestData()
				{
					supportedTypes = new List<Type> { typeof(decimal) },
					passingValues = new List<object> { dec },
					failingValues = new List<object> { null, string.Empty, " ", 101, decStr }
				},
				new CanSerializeTestData()
				{
					supportedTypes = new List<Type> { typeof(string) },
					passingValues = new List<object> { dec },
					failingValues = new List<object> { null, string.Empty, " ", 101, decStr }
				},
				new CanSerializeTestData()
				{
					supportedTypes = new List<Type> { typeof(decimal), typeof(string) },
					passingValues = new List<object> { dec },
					failingValues = new List<object> { null, string.Empty, " ", 101 }
				}
			});
		}

		[Test]
		public void CanDeserializeTests()
		{
			HashSet<Type> supportedTypes = new HashSet<Type>();
			TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
			DecimalProcessor processor = new DecimalProcessor(sd);

			decimal dec = 123.12345M;
			string decStr = dec.ToString(CultureInfo.InvariantCulture);

			GenericDeserializationProcessorTester<DecimalProcessor>.CanDeserializeTest(processor, new List<CanDeserializeTestData>()
			{
				new CanDeserializeTestData()
				{
					throwingValues = new List<DeserializationThrowingValue>()
					{
						new DeserializationThrowingValue(typeof(ArgumentNullException), null, null)
					},
					passingValues = new List<CanDeserializeValue>()
					{
						new CanDeserializeValue(typeof(decimal), dec),
						new CanDeserializeValue(typeof(decimal), decStr),
						new CanDeserializeValue(typeof(decimal), 101),
						new CanDeserializeValue(typeof(decimal), 101f),
						new CanDeserializeValue(typeof(decimal), 101.1),
					},
					failingValues = new List<CanDeserializeValue>()
					{
						new CanDeserializeValue(typeof(string), dec),
						new CanDeserializeValue(typeof(string), decStr),
						new CanDeserializeValue(typeof(int), dec),
					}
				}
			});
		}

		[Test]
		public void SerializeTest()
		{
			HashSet<Type> supportedTypes = new HashSet<Type>();
			TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
			DecimalProcessor processor = new DecimalProcessor(sd);

			decimal dec = 123.12345M;
			string decStr = dec.ToString(CultureInfo.InvariantCulture);

			GenericSerializationProcessorTester<DecimalProcessor>.SerializeTest(processor, supportedTypes, new[]
			{
				new SerializeTestData()
				{
					throwingValues = new List<SerializationThrowingValue>()
					{
						new SerializationThrowingValue(typeof(SerializationException), null),
						new SerializationThrowingValue(typeof(SerializationException), dec),
						new SerializationThrowingValue(typeof(SerializationException), decStr),
					}
				},
				new SerializeTestData()
				{
					supportedTypes = new List<Type>() { typeof(decimal) },
					passingValues = new List<SerializeValue>()
					{
						new SerializeValue(typeof(decimal), dec)
					},
					throwingValues = new List<SerializationThrowingValue>()
					{
						new SerializationThrowingValue(typeof(SerializationException), null),
						new SerializationThrowingValue(typeof(SerializationException), decStr),
						new SerializationThrowingValue(typeof(SerializationException), 101)
					}
				},
				new SerializeTestData()
				{
					supportedTypes = new List<Type>() { typeof(string) },
					passingValues = new List<SerializeValue>()
					{
						new SerializeValue(typeof(string), dec)
					},
					throwingValues = new List<SerializationThrowingValue>()
					{
						new SerializationThrowingValue(typeof(SerializationException), null),
						new SerializationThrowingValue(typeof(SerializationException), decStr),
						new SerializationThrowingValue(typeof(SerializationException), 101)
					}
				}
			});
		}

		[Test]
		public void DeserializeTest()
		{
			HashSet<Type> supportedTypes = new HashSet<Type>();
			TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
			DecimalProcessor processor = new DecimalProcessor(sd);

			decimal dec = 123.12345M;
			string decStr = dec.ToString(CultureInfo.InvariantCulture);

			GenericDeserializationProcessorTester<DecimalProcessor>.DeserializeTests(processor, new List<DeserializeTestData>()
			{
				new DeserializeTestData()
				{
					throwingValues = new List<DeserializationThrowingValue>()
					{
						new DeserializationThrowingValue(typeof(ArgumentNullException), null, null),
						new DeserializationThrowingValue(typeof(SerializationException), typeof(decimal), null),
						new DeserializationThrowingValue(typeof(SerializationException), typeof(string), dec),
						new DeserializationThrowingValue(typeof(SerializationException), typeof(decimal), "100.1.1"),
					},
					passingValues = new List<DeserializeValue>()
					{
						new DeserializeValue(typeof(decimal), dec),
						new DeserializeValue(typeof(decimal), decStr),
						new DeserializeValue(typeof(decimal), 101),
						new DeserializeValue(typeof(decimal), 101f),
						new DeserializeValue(typeof(decimal), 101.0),
						new DeserializeValue(typeof(decimal), "101"),
					}
				}
			});
		}
	}
}