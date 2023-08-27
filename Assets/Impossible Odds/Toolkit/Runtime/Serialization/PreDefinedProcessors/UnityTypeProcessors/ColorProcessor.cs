namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class ColorSequenceProcessor : UnityPrimitiveSequenceProcessor<Color>
	{
		private const int Size = 4;

		public ColorSequenceProcessor(IIndexSerializationDefinition definition)
		: base(definition)
		{ }

		/// <inheritdoc />
		public override bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			if (!base.CanDeserialize(targetType, dataToDeserialize))
			{
				return false;
			}

			return (dataToDeserialize is IList list) && (list.Count == Size);
		}

		protected override IList Serialize(Color value)
		{
			IList result = Definition.CreateSequenceInstance(Size);
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

		public ColorLookupProcessor(ILookupSerializationDefinition definition)
		: base(definition)
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
			IDictionary result = Definition.CreateLookupInstance(4);
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
		public ColorProcessor(IIndexSerializationDefinition sequenceDefinition, ILookupSerializationDefinition lookupDefinition, PrimitiveProcessingMethod preferredProcessingMethod)
		: this(new ColorSequenceProcessor(sequenceDefinition), new ColorLookupProcessor(lookupDefinition), preferredProcessingMethod)
		{ }

		public ColorProcessor(ColorSequenceProcessor sequenceProcessor, ColorLookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		: base(sequenceProcessor, lookupProcessor, preferredProcessingMethod)
		{ }
	}
}
