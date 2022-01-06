namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class Color32SequenceProcessor : UnityPrimitiveSequenceProcessor<Color32>
	{
		public Color32SequenceProcessor(IIndexSerializationDefinition definition)
		: base(definition)
		{ }

		protected override IList Serialize(Color32 value)
		{
			IList result = Definition.CreateSequenceInstance(4);
			result.Add(Serializer.Serialize(value.r, Definition));
			result.Add(Serializer.Serialize(value.g, Definition));
			result.Add(Serializer.Serialize(value.b, Definition));
			result.Add(Serializer.Serialize(value.a, Definition));
			return result;
		}

		protected override Color32 Deserialize(IList sequenceData)
		{
			return new Color32(
				Convert.ToByte(sequenceData[0]),
				Convert.ToByte(sequenceData[1]),
				Convert.ToByte(sequenceData[2]),
				Convert.ToByte(sequenceData[3]));
		}
	}

	public class Color32LookupProcessor : UnityPrimitiveLookupProcessor<Color32>
	{
		public Color32LookupProcessor(ILookupSerializationDefinition definition)
		: base(definition)
		{ }

		protected override IDictionary Serialize(Color32 value)
		{
			IDictionary result = Definition.CreateLookupInstance(4);
			result.Add(Serializer.Serialize("r", Definition), Serializer.Serialize(value.r, Definition));
			result.Add(Serializer.Serialize("g", Definition), Serializer.Serialize(value.g, Definition));
			result.Add(Serializer.Serialize("b", Definition), Serializer.Serialize(value.b, Definition));
			result.Add(Serializer.Serialize("a", Definition), Serializer.Serialize(value.a, Definition));

			return result;
		}

		protected override Color32 Deserialize(IDictionary lookupData)
		{
			return new Color32(
				Convert.ToByte(lookupData["r"]),
				Convert.ToByte(lookupData["g"]),
				Convert.ToByte(lookupData["b"]),
				Convert.ToByte(lookupData["a"]));
		}
	}

	public class Color32Processor : UnityPrimitiveSwitchProcessor<Color32SequenceProcessor, Color32LookupProcessor, Color32>
	{
		public Color32Processor(IIndexSerializationDefinition sequenceDefinition, ILookupSerializationDefinition lookupDefinition, PrimitiveProcessingMethod preferredProcessingMethod)
		: this(new Color32SequenceProcessor(sequenceDefinition), new Color32LookupProcessor(lookupDefinition), preferredProcessingMethod)
		{ }

		public Color32Processor(Color32SequenceProcessor sequenceProcessor, Color32LookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		: base(sequenceProcessor, lookupProcessor, preferredProcessingMethod)
		{ }
	}
}
