namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class Vector3IntSequenceProcessor : UnityPrimitiveSequenceProcessor<Vector3Int>
	{
		private const int Size = 3;

		public Vector3IntSequenceProcessor(IIndexSerializationDefinition definition)
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

		protected override Vector3Int Deserialize(IList sequenceData)
		{
			return new Vector3Int(
				Convert.ToInt32(sequenceData[0]),
				Convert.ToInt32(sequenceData[1]),
				Convert.ToInt32(sequenceData[2]));
		}

		protected override IList Serialize(Vector3Int value)
		{
			IList result = Definition.CreateSequenceInstance(Size);
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

		public Vector3IntLookupProcessor(ILookupSerializationDefinition definition)
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
			IDictionary result = Definition.CreateLookupInstance(3);
			result.Add(Serializer.Serialize(X, Definition), Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(Y, Definition), Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize(Z, Definition), Serializer.Serialize(value.z, Definition));
			return result;
		}
	}

	public class Vector3IntProcessor : UnityPrimitiveSwitchProcessor<Vector3IntSequenceProcessor, Vector3IntLookupProcessor, Vector3Int>
	{
		public Vector3IntProcessor(IIndexSerializationDefinition sequenceDefinition, ILookupSerializationDefinition lookupDefinition, PrimitiveProcessingMethod preferredProcessingMethod)
		: this(new Vector3IntSequenceProcessor(sequenceDefinition), new Vector3IntLookupProcessor(lookupDefinition), preferredProcessingMethod)
		{ }

		public Vector3IntProcessor(Vector3IntSequenceProcessor sequenceProcessor, Vector3IntLookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		: base(sequenceProcessor, lookupProcessor, preferredProcessingMethod)
		{ }
	}
}
