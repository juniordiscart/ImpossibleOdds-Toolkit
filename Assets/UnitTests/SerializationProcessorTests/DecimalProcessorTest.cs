using System;
using System.Collections.Generic;
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
			string decStr = dec.ToString();

			List<object> allTestData = new List<object>() { null, string.Empty, " ", 101, decStr, dec };
			List<object> onlyDecimalTestData = new List<object>() { dec };
			List<object> allButDecimalTestData = new List<object>() { null, string.Empty, " ", 101, decStr};

			GenericSerializationProcessorTester<DecimalProcessor>.CanSerializeTest(processor, supportedTypes, new List<CanSerializeTestData>()
			{
				new CanSerializeTestData()
				{
					failingValues = allTestData
				},
				new CanSerializeTestData()
				{
					supportedTypes = new List<Type>(){typeof(decimal)},
					passingValues = onlyDecimalTestData,
					failingValues = allButDecimalTestData
				},
				new CanSerializeTestData()
				{
					supportedTypes = new List<Type>() { typeof(decimal), typeof(string)},
					passingValues = onlyDecimalTestData,
					failingValues = allButDecimalTestData
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
			string decStr = dec.ToString();

			GenericDeserializationProcessorTester<DecimalProcessor>.CanDeserializeTest(processor, new List<CanDeserializeTestData>()
			{
				new CanDeserializeTestData()
				{
					throwingValues = new List<DeserializationThrowingValue>()
					{
						new DeserializationThrowingValue(typeof(ArgumentNullException), null, null)
					}
				}
			});
		}
	}
}