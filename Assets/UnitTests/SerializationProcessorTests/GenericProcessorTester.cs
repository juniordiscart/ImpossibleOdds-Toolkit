using System;
using System.Collections.Generic;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Processors;
using NUnit.Framework;

namespace Tests
{
	public class CanSerializeTestData
	{
		public List<Type> supportedTypes;
		public List<object> passingValues;
		public List<object> failingValues;
	}

	public class CanDeserializeTestData
	{
		public List<DeserializationThrowingValue> throwingValues;
		public List<CanDeserializeValue> passingValues;
		public List<CanDeserializeValue> failingValues;
	}

	public class SerializeTestData
	{
		public List<Type> supportedTypes;
		public List<SerializationThrowingValue> throwingValues;
		public List<SerializeValue> passingValues;
	}

	public class DeserializeTestData
	{
		public List<DeserializationThrowingValue> throwingValues;
		public List<DeserializeValue> passingValues;
	}

	public class CanDeserializeValue : Tuple<Type, object>
	{
		public CanDeserializeValue(Type targetType, object value) : base(targetType, value)
		{ }

		public Type TargetType => Item1;
		public object Value => Item2;
	}

	public class SerializeValue : Tuple<Type, object>
	{
		public SerializeValue(Type resultType, object value) : base(resultType, value)
		{ }

		public Type ResultType => Item1;
		public object Value => Item2;
	}

	public class SerializationThrowingValue : Tuple<Type, object>
	{
		public SerializationThrowingValue(Type exceptionType, object value) : base(exceptionType, value)
		{ }

		public Type ExceptionType => Item1;
		public object Value => Item2;
	}

	public class DeserializationThrowingValue : Tuple<Type, Type, object>
	{
		public DeserializationThrowingValue(Type exceptionType, Type targetType, object value) : base(exceptionType, targetType, value)
		{ }

		public Type ExceptionType => Item1;
		public Type TargetType => Item2;
		public object Value => Item3;
	}

	public class DeserializeValue : Tuple<Type, object>
	{
		public DeserializeValue(Type targetType, object value) : base(targetType, value)
		{ }

		public Type TargetType => Item1;
		public object Value => Item2;
	}

	public static class GenericSerializationProcessorTester<TProcessor>
	where TProcessor : ISerializationProcessor
	{
		public static void CanSerializeTest(TProcessor processor, HashSet<Type> supportedTypes, IEnumerable<CanSerializeTestData> testDataList)
		{
			if (testDataList == null)
			{
				return;
			}

			foreach(CanSerializeTestData testData in testDataList)
			{
				supportedTypes.Clear();
				if (testData.supportedTypes != null)
				{
					foreach(Type supportedDataType in testData.supportedTypes)
					{
						supportedTypes.Add(supportedDataType);
					}
				}

				if (testData.passingValues != null)
				{
					foreach (object passingTestValue in testData.passingValues)
					{
						Assert.IsTrue(processor.CanSerialize(passingTestValue));
					}
				}

				if (testData.failingValues != null)
				{
					foreach(object failingTestValue in testData.failingValues)
					{
						Assert.IsFalse(processor.CanSerialize(failingTestValue));
					}
				}
			}
		}

		public static void SerializeTest(TProcessor processor, HashSet<Type> supportedTypes, IEnumerable<SerializeTestData> testDataList)
		{
			if (testDataList == null)
			{
				return;
			}

			foreach(SerializeTestData testData in testDataList)
			{
				supportedTypes.Clear();
				if (testData.supportedTypes != null)
				{
					foreach (Type supportedDataType in testData.supportedTypes)
					{
						supportedTypes.Add(supportedDataType);
					}
				}

				if (testData.throwingValues != null)
				{
					foreach(SerializationThrowingValue throwingValue in testData.throwingValues)
					{
						Assert.Throws(throwingValue.ExceptionType, () => processor.Serialize(throwingValue.Value));
					}
				}

				if (testData.passingValues != null)
				{
					foreach(SerializeValue passingValue in testData.passingValues)
					{
						Assert.IsInstanceOf(passingValue.ResultType, processor.Serialize(passingValue.Value));
					}
				}
			}
		}
	}

	public static class GenericDeserializationProcessorTester<TProcessor>
	where TProcessor : IDeserializationProcessor
	{
		public static void CanDeserializeTest(TProcessor processor, IEnumerable<CanDeserializeTestData> testDataList)
		{
			if (testDataList == null)
			{
				return;
			}

			foreach(CanDeserializeTestData testData in testDataList)
			{
				if (testData.throwingValues != null)
				{
					foreach(DeserializationThrowingValue throwingValue in testData.throwingValues)
					{
						Assert.Throws(throwingValue.ExceptionType, () => processor.CanDeserialize(throwingValue.TargetType, throwingValue.Value));
					}
				}

				if (testData.passingValues != null)
				{
					foreach(CanDeserializeValue passingValue in testData.passingValues)
					{
						Assert.IsTrue(processor.CanDeserialize(passingValue.TargetType, passingValue.Value));
					}
				}

				if (testData.failingValues != null)
				{
					foreach(CanDeserializeValue failingValue in testData.failingValues)
					{
						Assert.IsFalse(processor.CanDeserialize(failingValue.TargetType, failingValue.Value));
					}
				}
			}
		}

		public static void DeserializeTests(TProcessor processor, IEnumerable<DeserializeTestData> testDataList)
		{
			if (testDataList == null)
			{
				return;
			}

			foreach(DeserializeTestData testData in testDataList)
			{
				if (testData.throwingValues != null)
				{
					foreach(DeserializationThrowingValue throwingValue in testData.throwingValues)
					{
						Assert.Throws(throwingValue.ExceptionType, () => processor.Deserialize(throwingValue.TargetType, throwingValue.Value));
					}
				}

				if (testData.passingValues != null)
				{
					foreach(DeserializeValue passingValue in testData.passingValues)
					{
						// There's no direct assertion to test whether a target type can be assigned null.
						// So a check is performed first to see if the result is null. If so, assert that the type
						// can accept null values. Otherwise, assert that the result is of the expected type.
						object result = processor.Deserialize(passingValue.TargetType, passingValue.Value);
						if (result == null)
						{
							Assert.IsTrue(SerializationUtilities.IsNullableType(passingValue.TargetType));
						}
						else
						{
							Assert.IsInstanceOf(passingValue.TargetType, result);
						}
					}
				}
			}
		}
	}
}