namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class Vector2IntSequenceProcessor : UnityPrimitiveSequenceProcessor<Vector2Int>
	{
		private const int Size = 2;

		public Vector2IntSequenceProcessor(IIndexSerializationDefinition definition)
		: base(definition)
		{ }

		/// <inheritdoc />
		public override bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			if (!CanDeserialize(targetType, dataToDeserialize))
			{
				return false;
			}

			return (dataToDeserialize is IList list) && (list.Count == Size);
		}

		protected override Vector2Int Deserialize(IList sequenceData)
		{
			return new Vector2Int(
				Convert.ToInt32(sequenceData[0]),
				Convert.ToInt32(sequenceData[1]));
		}

		protected override IList Serialize(Vector2Int value)
		{
			IList result = Definition.CreateSequenceInstance(Size);
			result.Add(Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(value.y, Definition));
			return result;
		}
	}

	public class Vector2IntLookupProcessor : UnityPrimitiveLookupProcessor<Vector2Int>
	{
		private const string X = "x";
		private const string Y = "y";

		public Vector2IntLookupProcessor(ILookupSerializationDefinition definition)
		: base(definition)
		{ }

		/// <inheritdoc />
		public override bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			if (!CanDeserialize(targetType, dataToDeserialize))
			{
				return false;
			}

			return (dataToDeserialize is IDictionary lookUp) && lookUp.Contains(X) && lookUp.Contains(Y);
		}

		protected override Vector2Int Deserialize(IDictionary lookupData)
		{
			return new Vector2Int(
				Convert.ToInt32(lookupData[X]),
				Convert.ToInt32(lookupData[Y]));
		}

		protected override IDictionary Serialize(Vector2Int value)
		{
			IDictionary result = Definition.CreateLookupInstance(2);
			result.Add(Serializer.Serialize(X, Definition), Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(Y, Definition), Serializer.Serialize(value.y, Definition));
			return result;
		}
	}

	public class Vector2IntProcessor : UnityPrimitiveSwitchProcessor<Vector2IntSequenceProcessor, Vector2IntLookupProcessor, Vector2Int>
	{
		public Vector2IntProcessor(IIndexSerializationDefinition sequenceDefinition, ILookupSerializationDefinition lookupDefinition, PrimitiveProcessingMethod preferredProcessingMethod)
		: this(new Vector2IntSequenceProcessor(sequenceDefinition), new Vector2IntLookupProcessor(lookupDefinition), preferredProcessingMethod)
		{ }

		public Vector2IntProcessor(Vector2IntSequenceProcessor sequenceProcessor, Vector2IntLookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		: base(sequenceProcessor, lookupProcessor, preferredProcessingMethod)
		{ }
	}
}
