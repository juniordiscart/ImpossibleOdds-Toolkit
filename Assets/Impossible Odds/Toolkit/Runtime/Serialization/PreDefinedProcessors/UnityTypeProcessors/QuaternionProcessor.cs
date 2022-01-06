namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class QuaternionSequenceProcessor : UnityPrimitiveSequenceProcessor<Quaternion>
	{
		public QuaternionSequenceProcessor(IIndexSerializationDefinition definition)
		: base(definition)
		{ }

		protected override Quaternion Deserialize(IList sequenceData)
		{
			return new Quaternion(
				Convert.ToSingle(sequenceData[0]),
				Convert.ToSingle(sequenceData[1]),
				Convert.ToSingle(sequenceData[2]),
				Convert.ToSingle(sequenceData[3]));
		}

		protected override IList Serialize(Quaternion value)
		{
			IList result = Definition.CreateSequenceInstance(4);
			result.Add(Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize(value.z, Definition));
			result.Add(Serializer.Serialize(value.w, Definition));
			return result;
		}
	}

	public class QuaternionLookupProcessor : UnityPrimitiveLookupProcessor<Quaternion>
	{
		public QuaternionLookupProcessor(ILookupSerializationDefinition definition)
		: base(definition)
		{ }

		protected override IDictionary Serialize(Quaternion value)
		{
			IDictionary result = Definition.CreateLookupInstance(4);
			result.Add(Serializer.Serialize("x", Definition), Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize("y", Definition), Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize("z", Definition), Serializer.Serialize(value.z, Definition));
			result.Add(Serializer.Serialize("w", Definition), Serializer.Serialize(value.w, Definition));
			return result;
		}

		protected override Quaternion Deserialize(IDictionary lookupData)
		{
			return new Quaternion(
				Convert.ToSingle(lookupData["x"]),
				Convert.ToSingle(lookupData["y"]),
				Convert.ToSingle(lookupData["z"]),
				Convert.ToSingle(lookupData["w"]));
		}
	}

	public class QuaternionProcessor : UnityPrimitiveSwitchProcessor<QuaternionSequenceProcessor, QuaternionLookupProcessor, Quaternion>
	{
		public QuaternionProcessor(IIndexSerializationDefinition sequenceDefinition, ILookupSerializationDefinition lookupDefinition, PrimitiveProcessingMethod preferredProcessingMethod)
		: this(new QuaternionSequenceProcessor(sequenceDefinition), new QuaternionLookupProcessor(lookupDefinition), preferredProcessingMethod)
		{ }

		public QuaternionProcessor(QuaternionSequenceProcessor sequenceProcessor, QuaternionLookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		: base(sequenceProcessor, lookupProcessor, preferredProcessingMethod)
		{ }
	}
}
