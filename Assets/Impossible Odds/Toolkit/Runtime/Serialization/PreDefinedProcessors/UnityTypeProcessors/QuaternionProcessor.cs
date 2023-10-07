using System;
using System.Collections;
using UnityEngine;

namespace ImpossibleOdds.Serialization.Processors
{
	public class QuaternionSequenceProcessor : UnityPrimitiveSequenceProcessor<Quaternion>
	{
		private const int Size = 4;

		public QuaternionSequenceProcessor(ISerializationDefinition definition, ISequenceSerializationConfiguration configuration)
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
			IList result = Configuration.CreateSequenceInstance(Size);
			result.Add(Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize(value.z, Definition));
			result.Add(Serializer.Serialize(value.w, Definition));
			return result;
		}
	}

	public class QuaternionLookupProcessor : UnityPrimitiveLookupProcessor<Quaternion>
	{
		private const string X = "x";
		private const string Y = "y";
		private const string Z = "z";
		private const string W = "w";

		public QuaternionLookupProcessor(ISerializationDefinition definition, ILookupSerializationConfiguration configuration)
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

		protected override IDictionary Serialize(Quaternion value)
		{
			IDictionary result = Configuration.CreateLookupInstance(4);
			result.Add(Serializer.Serialize(X, Definition), Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(Y, Definition), Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize(Z, Definition), Serializer.Serialize(value.z, Definition));
			result.Add(Serializer.Serialize(W, Definition), Serializer.Serialize(value.w, Definition));
			return result;
		}

		protected override Quaternion Deserialize(IDictionary lookupData)
		{
			return new Quaternion(
				Convert.ToSingle(lookupData[X]),
				Convert.ToSingle(lookupData[Y]),
				Convert.ToSingle(lookupData[Z]),
				Convert.ToSingle(lookupData[W]));
		}
	}

	public class QuaternionProcessor : UnityPrimitiveSwitchProcessor<QuaternionSequenceProcessor, QuaternionLookupProcessor, Quaternion>
	{
		public QuaternionProcessor(ISerializationDefinition definition, ISequenceSerializationConfiguration sequenceConfiguration, ILookupSerializationConfiguration lookupConfiguration, PrimitiveProcessingMethod preferredProcessingMethod)
		: this(new QuaternionSequenceProcessor(definition, sequenceConfiguration), new QuaternionLookupProcessor(definition, lookupConfiguration), preferredProcessingMethod)
		{ }

		public QuaternionProcessor(QuaternionSequenceProcessor sequenceProcessor, QuaternionLookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		: base(sequenceProcessor, lookupProcessor, preferredProcessingMethod)
		{ }
	}
}