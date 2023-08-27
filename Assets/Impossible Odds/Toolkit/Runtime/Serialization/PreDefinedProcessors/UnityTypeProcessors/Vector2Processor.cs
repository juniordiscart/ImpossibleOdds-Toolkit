namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class Vector2SequenceProcessor : UnityPrimitiveSequenceProcessor<Vector2>
	{
		private const int Size = 2;

		public Vector2SequenceProcessor(IIndexSerializationDefinition definition)
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

		protected override Vector2 Deserialize(IList sequenceData)
		{
			return new Vector2(
				Convert.ToSingle(sequenceData[0]),
				Convert.ToSingle(sequenceData[1]));
		}

		protected override IList Serialize(Vector2 value)
		{
			IList result = Definition.CreateSequenceInstance(Size);
			result.Add(Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(value.y, Definition));
			return result;
		}
	}

	public class Vector2LookupProcessor : UnityPrimitiveLookupProcessor<Vector2>
	{
		private const string X = "x";
		private const string Y = "y";

		public Vector2LookupProcessor(ILookupSerializationDefinition definition)
		: base(definition)
		{ }

		protected override IDictionary Serialize(Vector2 value)
		{
			IDictionary result = Definition.CreateLookupInstance(2);
			result.Add(Serializer.Serialize(X, Definition), Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(Y, Definition), Serializer.Serialize(value.y, Definition));
			return result;
		}

		protected override Vector2 Deserialize(IDictionary lookupData)
		{
			return new Vector2(
				Convert.ToSingle(lookupData[X]),
				Convert.ToSingle(lookupData[Y]));
		}
	}

	public class Vector2Processor : UnityPrimitiveSwitchProcessor<Vector2SequenceProcessor, Vector2LookupProcessor, Vector2>
	{
		public Vector2Processor(IIndexSerializationDefinition sequenceDefinition, ILookupSerializationDefinition lookupDefinition, PrimitiveProcessingMethod preferredProcessingMethod)
		: this(new Vector2SequenceProcessor(sequenceDefinition), new Vector2LookupProcessor(lookupDefinition), preferredProcessingMethod)
		{ }

		public Vector2Processor(Vector2SequenceProcessor sequenceProcessor, Vector2LookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		: base(sequenceProcessor, lookupProcessor, preferredProcessingMethod)
		{ }
	}
}
