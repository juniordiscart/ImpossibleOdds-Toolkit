using System;
using System.Collections;
using UnityEngine;

namespace ImpossibleOdds.Serialization.Processors
{
	public class Vector2SequenceProcessor : UnityPrimitiveSequenceProcessor<Vector2>
	{
		public Vector2SequenceProcessor(ISerializationDefinition definition, ISequenceSerializationConfiguration configuration)
		: base(definition, configuration)
		{ }

		public override int Size => 2;

		protected override Vector2 Deserialize(IList sequenceData)
		{
			return new Vector2(
				Convert.ToSingle(sequenceData[0]),
				Convert.ToSingle(sequenceData[1]));
		}

		protected override IList Serialize(Vector2 value)
		{
			IList result = Configuration.CreateSequenceInstance(Size);
			result.Add(Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(value.y, Definition));
			return result;
		}
	}

	public class Vector2LookupProcessor : UnityPrimitiveLookupProcessor<Vector2>
	{
		private static readonly string[] XY = { "x", "y" };

		public Vector2LookupProcessor(ISerializationDefinition definition, ILookupSerializationConfiguration configuration)
		: base(definition, configuration)
		{ }

		public override string[] Keys => XY;

		protected override IDictionary Serialize(Vector2 value)
		{
			IDictionary result = Configuration.CreateLookupInstance(2);
			result.Add(Serializer.Serialize(XY[0], Definition), Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(XY[1], Definition), Serializer.Serialize(value.y, Definition));
			return result;
		}

		protected override Vector2 Deserialize(IDictionary lookupData)
		{
			return new Vector2(
				Convert.ToSingle(lookupData[XY[0]]),
				Convert.ToSingle(lookupData[XY[1]]));
		}
	}

	public class Vector2Processor : UnityPrimitiveSwitchProcessor<Vector2SequenceProcessor, Vector2LookupProcessor, Vector2>
	{
		public Vector2Processor(ISerializationDefinition definition, ISequenceSerializationConfiguration sequenceConfiguration, ILookupSerializationConfiguration lookupConfiguration, PrimitiveProcessingMethod preferredProcessingMethod)
		: this(new Vector2SequenceProcessor(definition, sequenceConfiguration), new Vector2LookupProcessor(definition, lookupConfiguration), preferredProcessingMethod)
		{ }

		public Vector2Processor(Vector2SequenceProcessor sequenceProcessor, Vector2LookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		: base(sequenceProcessor, lookupProcessor, preferredProcessingMethod)
		{ }
	}
}