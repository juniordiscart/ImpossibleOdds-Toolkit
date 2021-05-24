namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class ColorSequenceProcessor : UnityPrimitiveSequenceProcessor<Color>
	{
		public ColorSequenceProcessor(IIndexSerializationDefinition definition)
		: base(definition)
		{ }

		protected override IList Serialize(Color value)
		{
			IList result = Definition.CreateSequenceInstance(4);
			result.Add(Serializer.Serialize(value.r, Definition));
			result.Add(Serializer.Serialize(value.g, Definition));
			result.Add(Serializer.Serialize(value.b, Definition));
			result.Add(Serializer.Serialize(value.a, Definition));
			return result;
		}

		protected override Color Deserialize(IList sequenceData)
		{
			return new Color(
				Convert.ToSingle(sequenceData[0]),
				Convert.ToSingle(sequenceData[1]),
				Convert.ToSingle(sequenceData[2]),
				Convert.ToSingle(sequenceData[3]));
		}
	}

	public class ColorLookupProcessor : UnityPrimitiveLookupProcessor<Color>
	{
		public ColorLookupProcessor(ILookupSerializationDefinition definition)
		: base(definition)
		{ }

		protected override IDictionary Serialize(Color value)
		{
			IDictionary result = Definition.CreateLookupInstance(4);
			result.Add(Serializer.Serialize("r", Definition), Serializer.Serialize(value.r, Definition));
			result.Add(Serializer.Serialize("g", Definition), Serializer.Serialize(value.g, Definition));
			result.Add(Serializer.Serialize("b", Definition), Serializer.Serialize(value.b, Definition));
			result.Add(Serializer.Serialize("a", Definition), Serializer.Serialize(value.a, Definition));
			return result;
		}

		protected override Color Deserialize(IDictionary lookupData)
		{
			return new Color(
				Convert.ToSingle(lookupData["r"]),
				Convert.ToSingle(lookupData["g"]),
				Convert.ToSingle(lookupData["b"]),
				Convert.ToSingle(lookupData["a"]));
		}
	}

	public class ColorProcessor : UnityPrimitiveSwitchProcessor<ColorSequenceProcessor, ColorLookupProcessor, Color>
	{
		public ColorProcessor(IIndexSerializationDefinition sequenceDefinition, ILookupSerializationDefinition lookupDefinition, PrimitiveProcessingMethod preferredProcessingMethod)
		: this(new ColorSequenceProcessor(sequenceDefinition), new ColorLookupProcessor(lookupDefinition), preferredProcessingMethod)
		{ }

		public ColorProcessor(ColorSequenceProcessor sequenceProcessor, ColorLookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		: base(sequenceProcessor, lookupProcessor, preferredProcessingMethod)
		{ }
	}
}
