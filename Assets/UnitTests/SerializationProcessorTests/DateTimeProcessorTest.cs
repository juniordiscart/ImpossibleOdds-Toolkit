using System;
using System.Collections.Generic;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Processors;
using NUnit.Framework;

namespace Tests
{
	public class DateTimeProcessorTest
	{
		[Test]
		public void CanSerializeTests()
		{
			HashSet<Type> supportedTypes = new HashSet<Type>();
			TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
			DateTimeProcessor processor = new DateTimeProcessor(sd);

			DateTime dateTime = DateTime.Now;
			string dateTimeStr = dateTime.ToString();

			List<object> allTestData = new List<object>() { null, string.Empty, " ", 101, dateTimeStr, dateTime };
			List<object> onlyDateTimeData = new List<object>() { dateTime };
			List<object> allButDateTimeTestData = new List<object>() { null, string.Empty, " ", 101, dateTimeStr};

			GenericSerializationProcessorTester<DateTimeProcessor>.CanSerializeTest(processor, supportedTypes, new List<CanSerializeTestData>()
			{
				// No supported types in the serialization definitions.
				new CanSerializeTestData()
				{
					failingValues = allTestData
				},
				// Serialization definition supports DateTime type.
				new CanSerializeTestData()
				{
					supportedTypes = new List<Type>() { typeof(DateTime) },
					passingValues = onlyDateTimeData,
					failingValues = allButDateTimeTestData
				},
				// Serialization definition supports string type.
				new CanSerializeTestData()
				{
					supportedTypes = new List<Type>() { typeof(string) },
					passingValues = onlyDateTimeData,
					failingValues = allButDateTimeTestData
				},
				// Serialization definition supports both DateTime and string types.
				new CanSerializeTestData()
				{
					supportedTypes = new List<Type>() { typeof(DateTime), typeof(string) },
					passingValues = onlyDateTimeData,
					failingValues = allButDateTimeTestData
				}
			});
		}

		[Test]
		public void CanDeserializeTests()
		{
			HashSet<Type> supportedTypes = new HashSet<Type>();
			TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
			DateTimeProcessor processor = new DateTimeProcessor(sd);

			DateTime dateTime = DateTime.Now;
			string dateTimeStr = dateTime.ToString();

			GenericDeserializationProcessorTester<DateTimeProcessor>.CanDeserializeTest(processor, new List<CanDeserializeTestData>()
			{
				new CanDeserializeTestData()
				{
					throwingValues = new List<DeserializationThrowingValue>()
					{
						new DeserializationThrowingValue(typeof(ArgumentNullException), null, null),
						new DeserializationThrowingValue(typeof(ArgumentNullException), null, dateTime),
						new DeserializationThrowingValue(typeof(ArgumentNullException), null, dateTimeStr),
					},
					passingValues = new List<CanDeserializeValue>()
					{
						new CanDeserializeValue(typeof(DateTime), string.Empty),
						new CanDeserializeValue(typeof(DateTime), " "),
						new CanDeserializeValue(typeof(DateTime), dateTimeStr),
						new CanDeserializeValue(typeof(DateTime), dateTime),
					},
					failingValues = new List<CanDeserializeValue>()
					{
						new CanDeserializeValue(typeof(string), dateTime),
						new CanDeserializeValue(typeof(DateTime), null),
						new CanDeserializeValue(typeof(DateTime), 101),
					}
				}
			});
		}

		[Test]
		public void SerializeTests()
		{
			HashSet<Type> supportedTypes = new HashSet<Type>();
			TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
			DateTimeProcessor processor = new DateTimeProcessor(sd);

			DateTime dateTime = DateTime.Now;
			string dateTimeStr = dateTime.ToString();
			string shortDateStr = dateTime.ToShortDateString();
			string shortTimeStr = dateTime.ToShortTimeString();

			GenericSerializationProcessorTester<DateTimeProcessor>.SerializeTest(processor, supportedTypes, new List<SerializeTestData>()
			{
				// No types supported by the serialization definition. Should throw everything.
				new SerializeTestData()
				{
					throwingValues = new List<SerializationThrowingValue>()
					{
						new SerializationThrowingValue(typeof(SerializationException), null),
						new SerializationThrowingValue(typeof(SerializationException), string.Empty),
						new SerializationThrowingValue(typeof(SerializationException), " "),
						new SerializationThrowingValue(typeof(SerializationException), 101),
						new SerializationThrowingValue(typeof(SerializationException), dateTime),
						new SerializationThrowingValue(typeof(SerializationException), dateTimeStr),
						new SerializationThrowingValue(typeof(SerializationException), shortDateStr),
						new SerializationThrowingValue(typeof(SerializationException), shortTimeStr),
					}
				},
				// Serialization definition supports the DateTime type.
				new SerializeTestData()
				{
					supportedTypes = new List<Type>() { typeof(DateTime) },
					passingValues = new List<SerializeValue>()
					{
						new SerializeValue(typeof(DateTime), dateTime)
					},
					throwingValues = new List<SerializationThrowingValue>()
					{
						new SerializationThrowingValue(typeof(SerializationException), null),
						new SerializationThrowingValue(typeof(SerializationException), string.Empty),
						new SerializationThrowingValue(typeof(SerializationException), " "),
						new SerializationThrowingValue(typeof(SerializationException), 101),
						new SerializationThrowingValue(typeof(SerializationException), dateTimeStr),
						new SerializationThrowingValue(typeof(SerializationException), shortDateStr),
						new SerializationThrowingValue(typeof(SerializationException), shortTimeStr),
					}
				},
				// Serialization definition supports the string type.
				new SerializeTestData()
				{
					supportedTypes = new List<Type>() { typeof(string) },
					passingValues = new List<SerializeValue>()
					{
						new SerializeValue(typeof(string), dateTime)
					},
					throwingValues = new List<SerializationThrowingValue>()
					{
						new SerializationThrowingValue(typeof(SerializationException), null),
						new SerializationThrowingValue(typeof(SerializationException), string.Empty),
						new SerializationThrowingValue(typeof(SerializationException), " "),
						new SerializationThrowingValue(typeof(SerializationException), 101),
						new SerializationThrowingValue(typeof(SerializationException), dateTimeStr),
						new SerializationThrowingValue(typeof(SerializationException), shortDateStr),
						new SerializationThrowingValue(typeof(SerializationException), shortTimeStr),
					}
				},
				// Serialization definition supports the string type.
				new SerializeTestData()
				{
					supportedTypes = new List<Type>() { typeof(DateTime), typeof(string) },
					passingValues = new List<SerializeValue>()
					{
						new SerializeValue(typeof(DateTime), dateTime)
					},
					throwingValues = new List<SerializationThrowingValue>()
					{
						new SerializationThrowingValue(typeof(SerializationException), null),
						new SerializationThrowingValue(typeof(SerializationException), string.Empty),
						new SerializationThrowingValue(typeof(SerializationException), " "),
						new SerializationThrowingValue(typeof(SerializationException), 101),
						new SerializationThrowingValue(typeof(SerializationException), dateTimeStr),
						new SerializationThrowingValue(typeof(SerializationException), shortDateStr),
						new SerializationThrowingValue(typeof(SerializationException), shortTimeStr),
					}
				}
			});
		}

		[Test]
		public void DeserializeTests()
		{
			HashSet<Type> supportedTypes = new HashSet<Type>();
			TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
			DateTimeProcessor processor = new DateTimeProcessor(sd);

			DateTime dateTime = DateTime.Now;
			string dateTimeStr = dateTime.ToString();
			string shortDateStr = dateTime.ToShortDateString();
			string shortTimeStr = dateTime.ToShortTimeString();

			GenericDeserializationProcessorTester<DateTimeProcessor>.DeserializeTests(processor, new List<DeserializeTestData>()
			{
				new DeserializeTestData()
				{
					throwingValues = new List<DeserializationThrowingValue>()
					{
						// Target type that is null should always throw and exception.
						new DeserializationThrowingValue(typeof(ArgumentNullException), null, null),
						// Null cannot be accepted as a value to deserialize.
						new DeserializationThrowingValue(typeof(SerializationException), typeof(DateTime), null),
						// String is not a valid target deserialization type.
						new DeserializationThrowingValue(typeof(SerializationException), typeof(string), null),
						// Invalid value to deserialize to a DateTime.
						new DeserializationThrowingValue(typeof(SerializationException), typeof(DateTime), 101),
						// Invalid strings.
						new DeserializationThrowingValue(typeof(FormatException), typeof(DateTime), string.Empty),
						new DeserializationThrowingValue(typeof(FormatException), typeof(DateTime), " "),
						new DeserializationThrowingValue(typeof(FormatException), typeof(DateTime), "ABCDE"),
					},
					passingValues = new List<DeserializeValue>()
					{
						new DeserializeValue(typeof(DateTime), DateTime.Now),
						new DeserializeValue(typeof(DateTime), dateTimeStr),
						new DeserializeValue(typeof(DateTime), shortDateStr),
						new DeserializeValue(typeof(DateTime), shortTimeStr),
					}
				}
			});
		}
	}
}