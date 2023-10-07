using System;
using System.Collections;
using UnityEngine;

namespace ImpossibleOdds.Serialization.Processors
{

	public class Vector3SequenceProcessor : UnityPrimitiveSequenceProcessor<Vector3>
	{
		private const int Size = 3;

		public Vector3SequenceProcessor(ISerializationDefinition definition, ISequenceSerializationConfiguration configuration)
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

		protected override Vector3 Deserialize(IList sequenceData)
		{
			return new Vector3(
				Convert.ToSingle(sequenceData[0]),
				Convert.ToSingle(sequenceData[1]),
				Convert.ToSingle(sequenceData[2]));
		}

		protected override IList Serialize(Vector3 value)
		{
			IList result = Configuration.CreateSequenceInstance(Size);
			result.Add(Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize(value.z, Definition));
			return result;
		}
	}

	public class Vector3LookupProcessor : UnityPrimitiveLookupProcessor<Vector3>
	{
		public const string X = "x";
		public const string Y = "y";
		public const string Z = "z";

		public Vector3LookupProcessor(ISerializationDefinition definition, ILookupSerializationConfiguration configuration)
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
				lookUp.Contains(X) && lookUp.Contains(Y) && lookUp.Contains(Z);
		}

		protected override IDictionary Serialize(Vector3 value)
		{
			IDictionary result = Configuration.CreateLookupInstance(3);
			result.Add(Serializer.Serialize(X, Definition), Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(Y, Definition), Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize(Z, Definition), Serializer.Serialize(value.z, Definition));
			return result;
		}

		protected override Vector3 Deserialize(IDictionary lookupData)
		{
			return new Vector3(
				Convert.ToSingle(lookupData[X]),
				Convert.ToSingle(lookupData[Y]),
				Convert.ToSingle(lookupData[Z]));
		}
	}

	public class Vector3Processor : UnityPrimitiveSwitchProcessor<Vector3SequenceProcessor, Vector3LookupProcessor, Vector3>
	{
		public Vector3Processor(ISerializationDefinition definition, ISequenceSerializationConfiguration sequenceConfiguration, ILookupSerializationConfiguration lookupConfiguration, PrimitiveProcessingMethod preferredProcessingMethod)
		: this(new Vector3SequenceProcessor(definition, sequenceConfiguration), new Vector3LookupProcessor(definition, lookupConfiguration), preferredProcessingMethod)
		{ }

		public Vector3Processor(Vector3SequenceProcessor sequenceProcessor, Vector3LookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		: base(sequenceProcessor, lookupProcessor, preferredProcessingMethod)
		{ }
	}
}