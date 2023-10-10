using System;
using System.Collections.Generic;
using ImpossibleOdds.Json;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Processors;
using NUnit.Framework;

namespace Tests
{
	public class EnumProcessorTest
	{
		public enum EnumPosition : byte
		{
			None,
			First,
			Second,
			Last
		}

		[JsonEnumString]
		public enum EnumDifficultyMode
		{
			Easy = 1,
			Normal = 2,
			Hard = 3
		}

		[Flags, JsonEnumString]
		public enum EnumGameModifierFlags
		{
			None = 0,
			[JsonEnumAlias("Mod1")]
			Modifier1 = 1 << 0,
			[JsonEnumAlias("Mod2")]
			Modifier2 = 1 << 1,
			Modifier3 = 1 << 2,
			[JsonEnumAlias("All")]
			AllModifiersActive = Modifier1 | Modifier2 | Modifier3
		}

		[Test]
		public void CanSerializeTest()
		{
			HashSet<Type> supportedTypes = new HashSet<Type>();
			TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
			EnumProcessor processor = new EnumProcessor(sd);
			IEnumAliasFeature enumAliasFeature = new EnumAliasFeature<JsonEnumStringAttribute, JsonEnumAliasAttribute>();

			// Test serialization without alias feature support
			GenericSerializationProcessorTester<EnumProcessor>.CanSerializeTest(processor, supportedTypes,
				new List<CanSerializeTestData>()
				{
					new CanSerializeTestData()
					{
						failingValues = new List<object>() { null, string.Empty, "123", 123, 123f },
						passingValues = new List<object>()
						{
							EnumPosition.First, EnumDifficultyMode.Easy, EnumGameModifierFlags.Modifier1
						}
					}
				});

			processor.AliasFeature = enumAliasFeature;

			// Test serialization with alias feature support
			GenericSerializationProcessorTester<EnumProcessor>.CanSerializeTest(processor, supportedTypes,
				new List<CanSerializeTestData>()
				{
					new CanSerializeTestData()
					{
						failingValues = new List<object>() { null, string.Empty, "123", 123, 123f },
						passingValues = new List<object>()
						{
							EnumPosition.First, EnumDifficultyMode.Easy, EnumGameModifierFlags.Modifier1
						}
					}
				});
		}

		[Test]
		public void CanDeserializeTest()
		{
			HashSet<Type> supportedTypes = new HashSet<Type>();
			TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
			EnumProcessor processor = new EnumProcessor(sd);
			IEnumAliasFeature enumAliasFeature = new EnumAliasFeature<JsonEnumStringAttribute, JsonEnumAliasAttribute>();

			// Test serialization without alias feature support
			GenericDeserializationProcessorTester<EnumProcessor>.CanDeserializeTest(processor,
				new List<CanDeserializeTestData>()
				{
					new CanDeserializeTestData()
					{
						throwingValues = new List<DeserializationThrowingValue>()
						{
							new DeserializationThrowingValue(typeof(ArgumentNullException), null, null),
							new DeserializationThrowingValue(typeof(ArgumentNullException), null, EnumPosition.First),
						},
						failingValues = new List<CanDeserializeValue>()
						{
							new CanDeserializeValue(typeof(int), EnumPosition.First),
							new CanDeserializeValue(typeof(string), EnumPosition.First),
							new CanDeserializeValue(typeof(string), EnumDifficultyMode.Easy),
							new CanDeserializeValue(typeof(string), EnumGameModifierFlags.Modifier1),
							new CanDeserializeValue(typeof(EnumDifficultyMode), null),
							new CanDeserializeValue(typeof(EnumDifficultyMode), 101f),
						},
						passingValues = new List<CanDeserializeValue>()
						{
							new CanDeserializeValue(typeof(EnumDifficultyMode), EnumDifficultyMode.Easy),
							new CanDeserializeValue(typeof(EnumGameModifierFlags), EnumDifficultyMode.Easy),
							new CanDeserializeValue(typeof(EnumDifficultyMode), 101),
							new CanDeserializeValue(typeof(EnumDifficultyMode), "ABC"),
							new CanDeserializeValue(typeof(EnumDifficultyMode), "Easy"),
						}
					}
				});

			processor.AliasFeature = enumAliasFeature;

			// Test serialization with alias feature support
			GenericDeserializationProcessorTester<EnumProcessor>.CanDeserializeTest(processor,
				new List<CanDeserializeTestData>()
				{
					new CanDeserializeTestData()
					{
						throwingValues = new List<DeserializationThrowingValue>()
						{
							new DeserializationThrowingValue(typeof(ArgumentNullException), null, null),
							new DeserializationThrowingValue(typeof(ArgumentNullException), null, EnumPosition.First),
						},
						failingValues = new List<CanDeserializeValue>()
						{
							new CanDeserializeValue(typeof(int), EnumPosition.First),
							new CanDeserializeValue(typeof(string), EnumPosition.First),
							new CanDeserializeValue(typeof(string), EnumDifficultyMode.Easy),
							new CanDeserializeValue(typeof(string), EnumGameModifierFlags.Modifier1),
							new CanDeserializeValue(typeof(EnumDifficultyMode), null),
							new CanDeserializeValue(typeof(EnumDifficultyMode), 101f),
						},
						passingValues = new List<CanDeserializeValue>()
						{
							new CanDeserializeValue(typeof(EnumDifficultyMode), EnumDifficultyMode.Easy),
							new CanDeserializeValue(typeof(EnumGameModifierFlags), EnumDifficultyMode.Easy),
							new CanDeserializeValue(typeof(EnumDifficultyMode), 101),
							new CanDeserializeValue(typeof(EnumDifficultyMode), "ABC"),
							new CanDeserializeValue(typeof(EnumDifficultyMode), "Easy"),
						}
					}
				});
		}

		[Test]
		public void SerializeTest()
		{
			HashSet<Type> supportedTypes = new HashSet<Type>();
			TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
			sd.SetProcessors(new IProcessor[]
			{
				new ExactMatchProcessor(sd), // Add this processor so that serialized values are accepted by the serialization definition.
				new PrimitiveTypeProcessor(sd) // Processing values to an appropriately supported type by the serialization definition.
			});

			IEnumAliasFeature enumAliasFeature = new EnumAliasFeature<JsonEnumStringAttribute, JsonEnumAliasAttribute>();
			EnumProcessor processor = new EnumProcessor(sd);

			// No enum alias support.
			GenericSerializationProcessorTester<EnumProcessor>.SerializeTest(processor, supportedTypes, new[]
			{
				new SerializeTestData()
				{
					throwingValues = new List<SerializationThrowingValue>()
					{
						new SerializationThrowingValue(typeof(SerializationException), null),
						new SerializationThrowingValue(typeof(SerializationException), 101),
						new SerializationThrowingValue(typeof(SerializationException), string.Empty),
						new SerializationThrowingValue(typeof(SerializationException), "ABC"),
						// The underlying value of the enum is not valid to the serialization definition.
						new SerializationThrowingValue(typeof(SerializationException), EnumPosition.First),
						new SerializationThrowingValue(typeof(SerializationException), EnumDifficultyMode.Easy),
						new SerializationThrowingValue(typeof(SerializationException), EnumGameModifierFlags.Modifier1),
					},
				},
				new SerializeTestData()
				{
					supportedTypes = new List<Type>() { typeof(string) },
					throwingValues = new List<SerializationThrowingValue>()
					{
						new SerializationThrowingValue(typeof(SerializationException), null),
						new SerializationThrowingValue(typeof(SerializationException), 101),
						new SerializationThrowingValue(typeof(SerializationException), string.Empty),
						new SerializationThrowingValue(typeof(SerializationException), "ABC"),
						// The underlying value of the enum is not valid to the serialization definition.
						new SerializationThrowingValue(typeof(SerializationException), EnumPosition.First),
						new SerializationThrowingValue(typeof(SerializationException), EnumDifficultyMode.Easy),
						new SerializationThrowingValue(typeof(SerializationException), EnumGameModifierFlags.Modifier1),
					},
				},
				new SerializeTestData()
				{
					supportedTypes = new List<Type>() { typeof(int) },
					throwingValues = new List<SerializationThrowingValue>()
					{
						new SerializationThrowingValue(typeof(SerializationException), null),
						new SerializationThrowingValue(typeof(SerializationException), 101),
						new SerializationThrowingValue(typeof(SerializationException), string.Empty),
						new SerializationThrowingValue(typeof(SerializationException), "ABC"),
					},
					passingValues = new List<SerializeValue>()
					{
						new SerializeValue(typeof(int), EnumPosition.First),
						new SerializeValue(typeof(int), EnumDifficultyMode.Easy),
						new SerializeValue(typeof(int), EnumGameModifierFlags.Modifier1)
					}
				},
				new SerializeTestData()
				{
					supportedTypes = new List<Type>() { typeof(byte) },
					throwingValues = new List<SerializationThrowingValue>()
					{
						new SerializationThrowingValue(typeof(SerializationException), null),
						new SerializationThrowingValue(typeof(SerializationException), 101),
						new SerializationThrowingValue(typeof(SerializationException), string.Empty),
						new SerializationThrowingValue(typeof(SerializationException), "ABC"),
						// The underlying type of these enum values cannot be converted down.
						new SerializationThrowingValue(typeof(SerializationException), EnumDifficultyMode.Easy),
						new SerializationThrowingValue(typeof(SerializationException), EnumGameModifierFlags.Modifier1),
					},
					passingValues = new List<SerializeValue>()
					{
						new SerializeValue(typeof(byte), EnumPosition.First),
					}
				},
				new SerializeTestData()
				{
					supportedTypes = new List<Type>() { typeof(byte), typeof(int), typeof(string) },
					throwingValues = new List<SerializationThrowingValue>()
					{
						new SerializationThrowingValue(typeof(SerializationException), null),
						new SerializationThrowingValue(typeof(SerializationException), 101),
						new SerializationThrowingValue(typeof(SerializationException), string.Empty),
						new SerializationThrowingValue(typeof(SerializationException), "ABC")
					},
					passingValues = new List<SerializeValue>()
					{
						new SerializeValue(typeof(byte), EnumPosition.First),
						new SerializeValue(typeof(int), EnumDifficultyMode.Easy),
						new SerializeValue(typeof(int), EnumGameModifierFlags.Modifier1),
					}
				},
			});

			// Adding enum alias support.
			processor.AliasFeature = enumAliasFeature;
			GenericSerializationProcessorTester<EnumProcessor>.SerializeTest(processor, supportedTypes, new[]
			{
				new SerializeTestData()
				{
					throwingValues = new List<SerializationThrowingValue>()
					{
						new SerializationThrowingValue(typeof(SerializationException), null),
						new SerializationThrowingValue(typeof(SerializationException), 101),
						new SerializationThrowingValue(typeof(SerializationException), string.Empty),
						new SerializationThrowingValue(typeof(SerializationException), "ABC"),
						// The underlying value of the enum is not valid to the serialization definition.
						new SerializationThrowingValue(typeof(SerializationException), EnumPosition.First),
						new SerializationThrowingValue(typeof(SerializationException), EnumDifficultyMode.Easy),
						new SerializationThrowingValue(typeof(SerializationException), EnumGameModifierFlags.Modifier1),
					},
				},
				new SerializeTestData()
				{
					supportedTypes = new List<Type>() { typeof(string) },
					throwingValues = new List<SerializationThrowingValue>()
					{
						new SerializationThrowingValue(typeof(SerializationException), null),
						new SerializationThrowingValue(typeof(SerializationException), 101),
						new SerializationThrowingValue(typeof(SerializationException), string.Empty),
						new SerializationThrowingValue(typeof(SerializationException), "ABC"),
						// The underlying value of the enum is not valid to the serialization definition.
						new SerializationThrowingValue(typeof(SerializationException), EnumPosition.First),
					},
					passingValues = new List<SerializeValue>()
					{
						new SerializeValue(typeof(string), EnumDifficultyMode.Easy),
						new SerializeValue(typeof(string), EnumGameModifierFlags.Modifier1)
					}
				},
				new SerializeTestData()
				{
					supportedTypes = new List<Type>() { typeof(int) },
					throwingValues = new List<SerializationThrowingValue>()
					{
						new SerializationThrowingValue(typeof(SerializationException), null),
						new SerializationThrowingValue(typeof(SerializationException), 101),
						new SerializationThrowingValue(typeof(SerializationException), string.Empty),
						new SerializationThrowingValue(typeof(SerializationException), "ABC"),
					},
					passingValues = new List<SerializeValue>()
					{
						new SerializeValue(typeof(int), EnumPosition.First), // Byte is not supported, so value is promoted to int.
						new SerializeValue(typeof(int), EnumDifficultyMode.Easy),
						new SerializeValue(typeof(int), EnumGameModifierFlags.Modifier1)
					}
				},
				new SerializeTestData()
				{
					supportedTypes = new List<Type>() { typeof(byte) },
					throwingValues = new List<SerializationThrowingValue>()
					{
						new SerializationThrowingValue(typeof(SerializationException), null),
						new SerializationThrowingValue(typeof(SerializationException), 101),
						new SerializationThrowingValue(typeof(SerializationException), string.Empty),
						new SerializationThrowingValue(typeof(SerializationException), "ABC"),
						// The underlying type of these enum values cannot be converted down.
						new SerializationThrowingValue(typeof(SerializationException), EnumDifficultyMode.Easy),
						new SerializationThrowingValue(typeof(SerializationException), EnumGameModifierFlags.Modifier1),
					},
					passingValues = new List<SerializeValue>()
					{
						new SerializeValue(typeof(byte), EnumPosition.First),
					}
				},
				new SerializeTestData()
				{
					supportedTypes = new List<Type>() { typeof(byte), typeof(int), typeof(string) },
					throwingValues = new List<SerializationThrowingValue>()
					{
						new SerializationThrowingValue(typeof(SerializationException), null),
						new SerializationThrowingValue(typeof(SerializationException), 101),
						new SerializationThrowingValue(typeof(SerializationException), string.Empty),
						new SerializationThrowingValue(typeof(SerializationException), "ABC")
					},
					passingValues = new List<SerializeValue>()
					{
						new SerializeValue(typeof(byte), EnumPosition.First),
						new SerializeValue(typeof(string), EnumDifficultyMode.Easy),
						new SerializeValue(typeof(string), EnumGameModifierFlags.Modifier1),
					}
				},
			});
		}

		[Test]
		public void DeserializeTest()
		{
			HashSet<Type> supportedTypes = new HashSet<Type>();
			TestSerializationDefinition sd = new TestSerializationDefinition(supportedTypes);
			sd.SetProcessors(new IProcessor[]
			{
				new ExactMatchProcessor(sd), // Add this processor so that serialized values are accepted by the serialization definition.
				new PrimitiveTypeProcessor(sd) // Processing values to an appropriately supported type by the serialization definition.
			});

			EnumProcessor processor = new EnumProcessor(sd);
			
			GenericDeserializationProcessorTester<EnumProcessor>.DeserializeTests(processor, new []
			{
				new DeserializeTestData()
				{
					throwingValues = new List<DeserializationThrowingValue>()
					{
						new DeserializationThrowingValue(typeof(ArgumentNullException), null, null),
						new DeserializationThrowingValue(typeof(SerializationException), typeof(Enum), null),
						new DeserializationThrowingValue(typeof(SerializationException), typeof(Enum), 101),
						new DeserializationThrowingValue(typeof(SerializationException), typeof(int), 101),
						new DeserializationThrowingValue(typeof(SerializationException), typeof(string), "ABC"),
						new DeserializationThrowingValue(typeof(SerializationException), typeof(EnumDifficultyMode), 101f),	// Not an integral type
						new DeserializationThrowingValue(typeof(SerializationException), typeof(EnumDifficultyMode), "Medium"),	// Unknown name
						new DeserializationThrowingValue(typeof(SerializationException), typeof(EnumGameModifierFlags), "Mod1"),	// Unknown name - no alias support
						new DeserializationThrowingValue(typeof(SerializationException), typeof(EnumGameModifierFlags), "Mod1, Mod2"), // Unknown names - no alias support
						new DeserializationThrowingValue(typeof(SerializationException), typeof(EnumGameModifierFlags), "Modifier1, Mod2"),	// Unknown name - no alias support.
					},
					passingValues = new List<DeserializeValue>()
					{
						new DeserializeValue(typeof(EnumDifficultyMode), -1),
						new DeserializeValue(typeof(EnumDifficultyMode), 1),
						new DeserializeValue(typeof(EnumDifficultyMode), "1"),
						new DeserializeValue(typeof(EnumDifficultyMode), "-1"),
						new DeserializeValue(typeof(EnumDifficultyMode), "Easy"),
						new DeserializeValue(typeof(EnumPosition), "None"),
						new DeserializeValue(typeof(EnumGameModifierFlags), 1 << 1),
						new DeserializeValue(typeof(EnumGameModifierFlags), "Modifier1"),
						new DeserializeValue(typeof(EnumGameModifierFlags), "Modifier1, Modifier2")
					}
				}
			});

			processor.AliasFeature = new EnumAliasFeature<JsonEnumStringAttribute, JsonEnumAliasAttribute>();
			GenericDeserializationProcessorTester<EnumProcessor>.DeserializeTests(processor, new []
			{
				new DeserializeTestData()
				{
					throwingValues = new List<DeserializationThrowingValue>()
					{
						new DeserializationThrowingValue(typeof(ArgumentNullException), null, null),
						new DeserializationThrowingValue(typeof(SerializationException), typeof(Enum), null),
						new DeserializationThrowingValue(typeof(SerializationException), typeof(Enum), 101),
						new DeserializationThrowingValue(typeof(SerializationException), typeof(int), 101),
						new DeserializationThrowingValue(typeof(SerializationException), typeof(string), "ABC"),
						new DeserializationThrowingValue(typeof(SerializationException), typeof(EnumDifficultyMode), "Medium"), // Unknown name
						new DeserializationThrowingValue(typeof(SerializationException), typeof(EnumDifficultyMode), 101f),	// Not an integral type
						new DeserializationThrowingValue(typeof(SerializationException), typeof(EnumGameModifierFlags), "Mod1, Mod2, Mod3"),	// Unknown alias
					},
					passingValues = new List<DeserializeValue>()
					{
						new DeserializeValue(typeof(EnumDifficultyMode), -1),	// Unknown value, but converted to anonymous value withing the enum
						new DeserializeValue(typeof(EnumDifficultyMode), "-1"),	// Unknown value, but converted to anonymous value withing the enum
						new DeserializeValue(typeof(EnumDifficultyMode), 1),
						new DeserializeValue(typeof(EnumDifficultyMode), "1"),
						new DeserializeValue(typeof(EnumDifficultyMode), "Easy"),
						new DeserializeValue(typeof(EnumPosition), "None"),
						new DeserializeValue(typeof(EnumGameModifierFlags), 1 << 1),
						new DeserializeValue(typeof(EnumGameModifierFlags), "Modifier1"),
						new DeserializeValue(typeof(EnumGameModifierFlags), "Mod1"),
						new DeserializeValue(typeof(EnumGameModifierFlags), "Mod1, Modifier2"),
						new DeserializeValue(typeof(EnumGameModifierFlags), "Mod1, Mod2, Modifier3"),
						new DeserializeValue(typeof(EnumGameModifierFlags), "All"),
					}
				}
			});
		}
	}
}