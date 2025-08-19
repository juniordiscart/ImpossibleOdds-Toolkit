using System;
using System.Collections;
using UnityEngine;

namespace ImpossibleOdds.Serialization.Processors
{
	public class Vector2IntSequenceProcessor : UnityPrimitiveSequenceProcessor<Vector2Int>
	{
		public Vector2IntSequenceProcessor(ISerializationDefinition definition, ISequenceSerializationConfiguration configuration)
		: base(definition, configuration)
		{ }

		public override int Size => 2;

		protected override Vector2Int Deserialize(IList sequenceData)
		{
			return new Vector2Int(
				Convert.ToInt32(sequenceData[0]),
				Convert.ToInt32(sequenceData[1]));
		}

		protected override IList Serialize(Vector2Int value)
		{
			IList result = Configuration.CreateSequenceInstance(Size);
			result.Add(Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(value.y, Definition));
			return result;
		}
	}

	public class Vector2IntLookupProcessor : UnityPrimitiveLookupProcessor<Vector2Int>
	{
		private static readonly string[] XY = { "x", "y" };

		public Vector2IntLookupProcessor(ISerializationDefinition definition, ILookupSerializationConfiguration configuration)
		: base(definition, configuration)
		{ }

		public override string[] Keys => XY;

		protected override Vector2Int Deserialize(IDictionary lookupData)
		{
			return new Vector2Int(
				Convert.ToInt32(lookupData[XY[0]]),
				Convert.ToInt32(lookupData[XY[1]]));
		}

		protected override IDictionary Serialize(Vector2Int value)
		{
			IDictionary result = Configuration.CreateLookupInstance(2);
			result.Add(Serializer.Serialize(XY[0], Definition), Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(XY[1], Definition), Serializer.Serialize(value.y, Definition));
			return result;
		}
	}

	public class Vector2IntProcessor : UnityPrimitiveSwitchProcessor<Vector2IntSequenceProcessor, Vector2IntLookupProcessor, Vector2Int>
	{
		public Vector2IntProcessor(ISerializationDefinition definition, ISequenceSerializationConfiguration sequenceConfiguration, ILookupSerializationConfiguration lookupConfiguration, PrimitiveProcessingMethod preferredProcessingMethod)
		: this(new Vector2IntSequenceProcessor(definition, sequenceConfiguration), new Vector2IntLookupProcessor(definition, lookupConfiguration), preferredProcessingMethod)
		{ }

		public Vector2IntProcessor(Vector2IntSequenceProcessor sequenceProcessor, Vector2IntLookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		: base(sequenceProcessor, lookupProcessor, preferredProcessingMethod)
		{ }
	}
}