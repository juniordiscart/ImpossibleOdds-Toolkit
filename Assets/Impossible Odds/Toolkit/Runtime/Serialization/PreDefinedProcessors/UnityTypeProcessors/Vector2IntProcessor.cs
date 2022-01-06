namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class Vector2IntSequenceProcessor : UnityPrimitiveSequenceProcessor<Vector2Int>
	{
		public Vector2IntSequenceProcessor(IIndexSerializationDefinition definition)
		: base(definition)
		{ }

		protected override Vector2Int Deserialize(IList sequenceData)
		{
			return new Vector2Int(
				Convert.ToInt32(sequenceData[0]),
				Convert.ToInt32(sequenceData[1]));
		}

		protected override IList Serialize(Vector2Int value)
		{
			IList result = Definition.CreateSequenceInstance(2);
			result.Add(Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(value.y, Definition));
			return result;
		}
	}

	public class Vector2IntLookupProcessor : UnityPrimitiveLookupProcessor<Vector2Int>
	{
		public Vector2IntLookupProcessor(ILookupSerializationDefinition definition)
		: base(definition)
		{ }

		protected override Vector2Int Deserialize(IDictionary lookupData)
		{
			return new Vector2Int(
				Convert.ToInt32(lookupData["x"]),
				Convert.ToInt32(lookupData["y"]));
		}

		protected override IDictionary Serialize(Vector2Int value)
		{
			IDictionary result = Definition.CreateLookupInstance(2);
			result.Add(Serializer.Serialize("x", Definition), Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize("y", Definition), Serializer.Serialize(value.y, Definition));
			return result;
		}
	}

	public class Vector2IntProcessor : UnityPrimitiveSwitchProcessor<Vector2IntSequenceProcessor, Vector2IntLookupProcessor, Vector2Int>
	{
		public Vector2IntProcessor(IIndexSerializationDefinition sequenceDefinition, ILookupSerializationDefinition lookupDefinition, PrimitiveProcessingMethod preferredProcessingMethod)
		: this(new Vector2IntSequenceProcessor(sequenceDefinition), new Vector2IntLookupProcessor(lookupDefinition), preferredProcessingMethod)
		{ }

		public Vector2IntProcessor(Vector2IntSequenceProcessor sequenceProcessor, Vector2IntLookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		: base(sequenceProcessor, lookupProcessor, preferredProcessingMethod)
		{ }
	}
}
