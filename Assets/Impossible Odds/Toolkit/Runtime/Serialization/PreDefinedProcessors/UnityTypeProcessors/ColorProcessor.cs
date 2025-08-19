using System;
using System.Collections;
using UnityEngine;

namespace ImpossibleOdds.Serialization.Processors
{
	public class ColorSequenceProcessor : UnityPrimitiveSequenceProcessor<Color>
	{
		public ColorSequenceProcessor(ISerializationDefinition definition, ISequenceSerializationConfiguration configuration)
		: base(definition, configuration)
		{ }
		
		public override int Size => 4;

		protected override IList Serialize(Color value)
		{
			IList result = Configuration.CreateSequenceInstance(Size);
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
		private static readonly string[] RGBA = { "r", "g", "b", "a" };

		public ColorLookupProcessor(ISerializationDefinition definition, ILookupSerializationConfiguration configuration)
		: base(definition, configuration)
		{ }

		public override string[] Keys => RGBA;
		
		protected override IDictionary Serialize(Color value)
		{
			IDictionary result = Configuration.CreateLookupInstance(4);
			result.Add(Serializer.Serialize(RGBA[0], Definition), Serializer.Serialize(value.r, Definition));
			result.Add(Serializer.Serialize(RGBA[1], Definition), Serializer.Serialize(value.g, Definition));
			result.Add(Serializer.Serialize(RGBA[2], Definition), Serializer.Serialize(value.b, Definition));
			result.Add(Serializer.Serialize(RGBA[3], Definition), Serializer.Serialize(value.a, Definition));
			return result;
		}

		protected override Color Deserialize(IDictionary lookupData)
		{
			return new Color(
				Convert.ToSingle(lookupData[RGBA[0]]),
				Convert.ToSingle(lookupData[RGBA[1]]),
				Convert.ToSingle(lookupData[RGBA[2]]),
				Convert.ToSingle(lookupData[RGBA[3]]));
		}
	}

	public class ColorProcessor : UnityPrimitiveSwitchProcessor<ColorSequenceProcessor, ColorLookupProcessor, Color>
	{
		public ColorProcessor(ISerializationDefinition definition, ISequenceSerializationConfiguration sequenceConfiguration, ILookupSerializationConfiguration lookupConfiguration, PrimitiveProcessingMethod preferredProcessingMethod)
		: this(new ColorSequenceProcessor(definition, sequenceConfiguration), new ColorLookupProcessor(definition, lookupConfiguration), preferredProcessingMethod)
		{ }

		public ColorProcessor(ColorSequenceProcessor sequenceProcessor, ColorLookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		: base(sequenceProcessor, lookupProcessor, preferredProcessingMethod)
		{ }
	}
}