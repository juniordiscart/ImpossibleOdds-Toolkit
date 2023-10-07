using System;
using System.Collections;
using UnityEngine;

namespace ImpossibleOdds.Serialization.Processors
{
	public class Color32SequenceProcessor : UnityPrimitiveSequenceProcessor<Color32>
	{
		private const int Size = 4;

		public Color32SequenceProcessor(ISerializationDefinition definition, ISequenceSerializationConfiguration configuration)
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

		protected override IList Serialize(Color32 value)
		{
			IList result = Configuration.CreateSequenceInstance(Size);
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
		private const string R = "r";
		private const string G = "g";
		private const string B = "b";
		private const string A = "a";

		public Color32LookupProcessor(ISerializationDefinition definition, ILookupSerializationConfiguration configuration)
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

		protected override IDictionary Serialize(Color32 value)
		{
			IDictionary result = Configuration.CreateLookupInstance(4);
			result.Add(Serializer.Serialize(R, Definition), Serializer.Serialize(value.r, Definition));
			result.Add(Serializer.Serialize(G, Definition), Serializer.Serialize(value.g, Definition));
			result.Add(Serializer.Serialize(B, Definition), Serializer.Serialize(value.b, Definition));
			result.Add(Serializer.Serialize(A, Definition), Serializer.Serialize(value.a, Definition));

			return result;
		}

		protected override Color32 Deserialize(IDictionary lookupData)
		{
			return new Color32(
				Convert.ToByte(lookupData[R]),
				Convert.ToByte(lookupData[G]),
				Convert.ToByte(lookupData[B]),
				Convert.ToByte(lookupData[A]));
		}
	}

	public class Color32Processor : UnityPrimitiveSwitchProcessor<Color32SequenceProcessor, Color32LookupProcessor, Color32>
	{
		public Color32Processor(ISerializationDefinition definition, ISequenceSerializationConfiguration sequenceConfiguration, ILookupSerializationConfiguration lookupConfiguration, PrimitiveProcessingMethod preferredProcessingMethod)
		: this(new Color32SequenceProcessor(definition, sequenceConfiguration), new Color32LookupProcessor(definition, lookupConfiguration), preferredProcessingMethod)
		{ }

		public Color32Processor(Color32SequenceProcessor sequenceProcessor, Color32LookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		: base(sequenceProcessor, lookupProcessor, preferredProcessingMethod)
		{ }
	}
}