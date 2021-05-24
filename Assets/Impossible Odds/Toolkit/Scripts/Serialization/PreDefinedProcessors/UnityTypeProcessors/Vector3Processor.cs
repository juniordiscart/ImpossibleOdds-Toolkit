namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class Vector3SequenceProcessor : UnityPrimitiveSequenceProcessor<Vector3>
	{
		public Vector3SequenceProcessor(IIndexSerializationDefinition definition)
		: base(definition)
		{ }

		protected override Vector3 Deserialize(IList sequenceData)
		{
			return new Vector3(
				Convert.ToSingle(sequenceData[0]),
				Convert.ToSingle(sequenceData[1]),
				Convert.ToSingle(sequenceData[2]));
		}

		protected override IList Serialize(Vector3 value)
		{
			IList result = Definition.CreateSequenceInstance(3);
			result.Add(Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize(value.z, Definition));
			return result;
		}
	}

	public class Vector3LookupProcessor : UnityPrimitiveLookupProcessor<Vector3>
	{
		public Vector3LookupProcessor(ILookupSerializationDefinition definition)
		: base(definition)
		{ }

		protected override IDictionary Serialize(Vector3 value)
		{
			IDictionary result = Definition.CreateLookupInstance(3);
			result.Add(Serializer.Serialize("x", Definition), Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize("y", Definition), Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize("z", Definition), Serializer.Serialize(value.z, Definition));
			return result;
		}

		protected override Vector3 Deserialize(IDictionary lookupData)
		{
			return new Vector3(
				Convert.ToSingle(lookupData["x"]),
				Convert.ToSingle(lookupData["y"]),
				Convert.ToSingle(lookupData["z"]));
		}
	}

	public class Vector3Processor : UnityPrimitiveSwitchProcessor<Vector3SequenceProcessor, Vector3LookupProcessor, Vector3>
	{
		public Vector3Processor(IIndexSerializationDefinition sequenceDefinition, ILookupSerializationDefinition lookupDefinition, PrimitiveProcessingMethod preferredProcessingMethod)
		: this(new Vector3SequenceProcessor(sequenceDefinition), new Vector3LookupProcessor(lookupDefinition), preferredProcessingMethod)
		{ }

		public Vector3Processor(Vector3SequenceProcessor sequenceProcessor, Vector3LookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		: base(sequenceProcessor, lookupProcessor, preferredProcessingMethod)
		{ }
	}
}
