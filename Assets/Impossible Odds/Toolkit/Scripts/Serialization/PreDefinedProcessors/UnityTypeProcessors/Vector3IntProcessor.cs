namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class Vector3IntSequenceProcessor : UnityPrimitiveSequenceProcessor<Vector3Int>
	{
		public Vector3IntSequenceProcessor(IIndexSerializationDefinition definition)
		: base(definition)
		{ }

		protected override Vector3Int Deserialize(IList sequenceData)
		{
			return new Vector3Int(
				Convert.ToInt32(sequenceData[0]),
				Convert.ToInt32(sequenceData[1]),
				Convert.ToInt32(sequenceData[2]));
		}

		protected override IList Serialize(Vector3Int value)
		{
			IList result = Definition.CreateSequenceInstance(3);
			result.Add(Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize(value.z, Definition));
			return result;
		}
	}

	public class Vector3IntLookupProcessor : UnityPrimitiveLookupProcessor<Vector3Int>
	{
		public Vector3IntLookupProcessor(ILookupSerializationDefinition definition)
		: base(definition)
		{ }

		protected override Vector3Int Deserialize(IDictionary lookupData)
		{
			return new Vector3Int(
				Convert.ToInt32(lookupData["x"]),
				Convert.ToInt32(lookupData["y"]),
				Convert.ToInt32(lookupData["z"]));
		}

		protected override IDictionary Serialize(Vector3Int value)
		{
			IDictionary result = Definition.CreateLookupInstance(3);
			result.Add(Serializer.Serialize("x", Definition), Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize("y", Definition), Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize("z", Definition), Serializer.Serialize(value.z, Definition));
			return result;
		}
	}

	public class Vector3IntProcessor : UnityPrimitiveSwitchProcessor<Vector3IntSequenceProcessor, Vector3IntLookupProcessor, Vector3Int>
	{
		public Vector3IntProcessor(IIndexSerializationDefinition sequenceDefinition, ILookupSerializationDefinition lookupDefinition, PrimitiveProcessingMethod preferredProcessingMethod)
		: this(new Vector3IntSequenceProcessor(sequenceDefinition), new Vector3IntLookupProcessor(lookupDefinition), preferredProcessingMethod)
		{ }

		public Vector3IntProcessor(Vector3IntSequenceProcessor sequenceProcessor, Vector3IntLookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		: base(sequenceProcessor, lookupProcessor, preferredProcessingMethod)
		{ }
	}
}
