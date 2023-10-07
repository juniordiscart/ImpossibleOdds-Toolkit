using System;
using System.Collections;
using UnityEngine;

namespace ImpossibleOdds.Serialization.Processors
{
	public class Vector3IntSequenceProcessor : UnityPrimitiveSequenceProcessor<Vector3Int>
	{
		private const int Size = 3;

		public Vector3IntSequenceProcessor(ISerializationDefinition definition, ISequenceSerializationConfiguration configuration)
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

		protected override Vector3Int Deserialize(IList sequenceData)
		{
			return new Vector3Int(
				Convert.ToInt32(sequenceData[0]),
				Convert.ToInt32(sequenceData[1]),
				Convert.ToInt32(sequenceData[2]));
		}

		protected override IList Serialize(Vector3Int value)
		{
			IList result = Configuration.CreateSequenceInstance(Size);
			result.Add(Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize(value.z, Definition));
			return result;
		}
	}

	public class Vector3IntLookupProcessor : UnityPrimitiveLookupProcessor<Vector3Int>
	{
		private const string X = "x";
		private const string Y = "y";
		private const string Z = "z";

		public Vector3IntLookupProcessor(ISerializationDefinition definition, ILookupSerializationConfiguration configuration)
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

		protected override Vector3Int Deserialize(IDictionary lookupData)
		{
			return new Vector3Int(
				Convert.ToInt32(lookupData[X]),
				Convert.ToInt32(lookupData[Y]),
				Convert.ToInt32(lookupData[Z]));
		}

		protected override IDictionary Serialize(Vector3Int value)
		{
			IDictionary result = Configuration.CreateLookupInstance(3);
			result.Add(Serializer.Serialize(X, Definition), Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(Y, Definition), Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize(Z, Definition), Serializer.Serialize(value.z, Definition));
			return result;
		}
	}

	public class Vector3IntProcessor : UnityPrimitiveSwitchProcessor<Vector3IntSequenceProcessor, Vector3IntLookupProcessor, Vector3Int>
	{
		public Vector3IntProcessor(ISerializationDefinition definition, ISequenceSerializationConfiguration sequenceConfiguration, ILookupSerializationConfiguration lookupConfiguration, PrimitiveProcessingMethod preferredProcessingMethod)
		: this(new Vector3IntSequenceProcessor(definition, sequenceConfiguration), new Vector3IntLookupProcessor(definition, lookupConfiguration), preferredProcessingMethod)
		{ }

		public Vector3IntProcessor(Vector3IntSequenceProcessor sequenceProcessor, Vector3IntLookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		: base(sequenceProcessor, lookupProcessor, preferredProcessingMethod)
		{ }
	}
}