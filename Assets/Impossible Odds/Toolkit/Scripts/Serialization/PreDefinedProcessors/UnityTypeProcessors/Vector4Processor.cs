namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class Vector4SequenceProcessor : UnityPrimitiveSequenceProcessor<Vector4>
	{
		public Vector4SequenceProcessor(IIndexSerializationDefinition definition)
		: base(definition)
		{ }

		protected override Vector4 Deserialize(IList sequenceData)
		{
			return new Vector4(
				Convert.ToSingle(sequenceData[0]),
				Convert.ToSingle(sequenceData[1]),
				Convert.ToSingle(sequenceData[2]),
				Convert.ToSingle(sequenceData[3]));
		}

		protected override IList Serialize(Vector4 value)
		{
			IList result = Definition.CreateSequenceInstance(4);
			result.Add(Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize(value.z, Definition));
			result.Add(Serializer.Serialize(value.w, Definition));
			return result;
		}
	}

	public class Vector4LookupProcessor : UnityPrimitiveLookupProcessor<Vector4>
	{
		public Vector4LookupProcessor(ILookupSerializationDefinition definition)
		: base(definition)
		{ }

		protected override IDictionary Serialize(Vector4 value)
		{
			IDictionary result = Definition.CreateLookupInstance(4);
			result.Add(Serializer.Serialize("x", Definition), Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize("y", Definition), Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize("z", Definition), Serializer.Serialize(value.z, Definition));
			result.Add(Serializer.Serialize("w", Definition), Serializer.Serialize(value.z, Definition));
			return result;
		}

		protected override Vector4 Deserialize(IDictionary lookupData)
		{
			return new Vector4(
				Convert.ToSingle(lookupData["x"]),
				Convert.ToSingle(lookupData["y"]),
				Convert.ToSingle(lookupData["z"]),
				Convert.ToSingle(lookupData["w"]));
		}
	}

	public class Vector4Processor : UnityPrimitiveSwitchProcessor<Vector4SequenceProcessor, Vector4LookupProcessor, Vector4>
	{
		public Vector4Processor(IIndexSerializationDefinition sequenceDefinition, ILookupSerializationDefinition lookupDefinition, PrimitiveProcessingMethod preferredProcessingMethod)
		: this(new Vector4SequenceProcessor(sequenceDefinition), new Vector4LookupProcessor(lookupDefinition), preferredProcessingMethod)
		{ }

		public Vector4Processor(Vector4SequenceProcessor sequenceProcessor, Vector4LookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		: base(sequenceProcessor, lookupProcessor, preferredProcessingMethod)
		{ }
	}
}
