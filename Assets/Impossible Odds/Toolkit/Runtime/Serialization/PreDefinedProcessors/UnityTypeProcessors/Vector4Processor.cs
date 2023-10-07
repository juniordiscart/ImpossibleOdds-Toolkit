using System;
using System.Collections;
using UnityEngine;

namespace ImpossibleOdds.Serialization.Processors
{
	public class Vector4SequenceProcessor : UnityPrimitiveSequenceProcessor<Vector4>
	{
		public const int Size = 4;

		public Vector4SequenceProcessor(ISerializationDefinition definition, ISequenceSerializationConfiguration configuration)
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
			IList result = Configuration.CreateSequenceInstance(Size);
			result.Add(Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize(value.z, Definition));
			result.Add(Serializer.Serialize(value.w, Definition));
			return result;
		}
	}

	public class Vector4LookupProcessor : UnityPrimitiveLookupProcessor<Vector4>
	{
		private const string X = "x";
		private const string Y = "y";
		private const string Z = "z";
		private const string W = "w";

		public Vector4LookupProcessor(ISerializationDefinition definition, ILookupSerializationConfiguration configuration)
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
				lookUp.Contains(X) && lookUp.Contains(Y) &&
				lookUp.Contains(Z) && lookUp.Contains(W);
		}

		protected override IDictionary Serialize(Vector4 value)
		{
			IDictionary result = Configuration.CreateLookupInstance(4);
			result.Add(Serializer.Serialize(X, Definition), Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(Y, Definition), Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize(Z, Definition), Serializer.Serialize(value.z, Definition));
			result.Add(Serializer.Serialize(W, Definition), Serializer.Serialize(value.w, Definition));
			return result;
		}

		protected override Vector4 Deserialize(IDictionary lookupData)
		{
			return new Vector4(
				Convert.ToSingle(lookupData[X]),
				Convert.ToSingle(lookupData[Y]),
				Convert.ToSingle(lookupData[Z]),
				Convert.ToSingle(lookupData[W]));
		}
	}

	public class Vector4Processor : UnityPrimitiveSwitchProcessor<Vector4SequenceProcessor, Vector4LookupProcessor, Vector4>
	{
		public Vector4Processor(ISerializationDefinition definition, ISequenceSerializationConfiguration sequenceConfiguration, ILookupSerializationConfiguration lookupConfiguration, PrimitiveProcessingMethod preferredProcessingMethod)
		: this(new Vector4SequenceProcessor(definition, sequenceConfiguration), new Vector4LookupProcessor(definition, lookupConfiguration), preferredProcessingMethod)
		{ }

		public Vector4Processor(Vector4SequenceProcessor sequenceProcessor, Vector4LookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		: base(sequenceProcessor, lookupProcessor, preferredProcessingMethod)
		{ }
	}
}