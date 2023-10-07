using System;
using System.Collections;
using UnityEngine;

namespace ImpossibleOdds.Serialization.Processors
{
	public class ColorSequenceProcessor : UnityPrimitiveSequenceProcessor<Color>
	{
		private const int Size = 4;

		public ColorSequenceProcessor(ISerializationDefinition definition, ISequenceSerializationConfiguration configuration)
		: base(definition, configuration)
		{ }

		/// <inheritdoc />
		public override bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			if (!base.CanDeserialize(targetType, dataToDeserialize))
			{
				return false;
			}

			return (dataToDeserialize is IList { Count: Size });
		}

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
		private const string R = "r";
		private const string G = "g";
		private const string B = "b";
		private const string A = "a";

		public ColorLookupProcessor(ISerializationDefinition definition, ILookupSerializationConfiguration configuration)
		: base(definition, configuration)
		{ }

		/// <inheritdoc />
		public override bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			if (!base.CanDeserialize(targetType, dataToDeserialize))
			{
				return false;
			}

			return
				(dataToDeserialize is IDictionary lookUp) &&
				lookUp.Contains(R) && lookUp.Contains(G) &&
				lookUp.Contains(B) && lookUp.Contains(A);
		}

		protected override IDictionary Serialize(Color value)
		{
			IDictionary result = Configuration.CreateLookupInstance(4);
			result.Add(Serializer.Serialize(R, Definition), Serializer.Serialize(value.r, Definition));
			result.Add(Serializer.Serialize(G, Definition), Serializer.Serialize(value.g, Definition));
			result.Add(Serializer.Serialize(B, Definition), Serializer.Serialize(value.b, Definition));
			result.Add(Serializer.Serialize(A, Definition), Serializer.Serialize(value.a, Definition));
			return result;
		}

		protected override Color Deserialize(IDictionary lookupData)
		{
			return new Color(
				Convert.ToSingle(lookupData[R]),
				Convert.ToSingle(lookupData[G]),
				Convert.ToSingle(lookupData[B]),
				Convert.ToSingle(lookupData[A]));
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