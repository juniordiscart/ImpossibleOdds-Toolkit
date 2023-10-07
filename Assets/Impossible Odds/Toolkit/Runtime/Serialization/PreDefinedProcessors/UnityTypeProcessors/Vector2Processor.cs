using System;
using System.Collections;
using UnityEngine;

namespace ImpossibleOdds.Serialization.Processors
{
	public class Vector2SequenceProcessor : UnityPrimitiveSequenceProcessor<Vector2>
	{
		private const int Size = 2;

		public Vector2SequenceProcessor(ISerializationDefinition definition, ISequenceSerializationConfiguration configuration)
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

		protected override Vector2 Deserialize(IList sequenceData)
		{
			return new Vector2(
				Convert.ToSingle(sequenceData[0]),
				Convert.ToSingle(sequenceData[1]));
		}

		protected override IList Serialize(Vector2 value)
		{
			IList result = Configuration.CreateSequenceInstance(Size);
			result.Add(Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(value.y, Definition));
			return result;
		}
	}

	public class Vector2LookupProcessor : UnityPrimitiveLookupProcessor<Vector2>
	{
		private const string X = "x";
		private const string Y = "y";

		public Vector2LookupProcessor(ISerializationDefinition definition, ILookupSerializationConfiguration configuration)
		: base(definition, configuration)
		{ }

		/// <inheritdoc />
		public override bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			if (!base.CanDeserialize(targetType, dataToDeserialize))
			{
				return false;
			}

			return (dataToDeserialize is IDictionary lookUp) && lookUp.Contains(X) && lookUp.Contains(Y);
		}

		protected override IDictionary Serialize(Vector2 value)
		{
			IDictionary result = Configuration.CreateLookupInstance(2);
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
		public Vector2Processor(ISerializationDefinition definition, ISequenceSerializationConfiguration sequenceConfiguration, ILookupSerializationConfiguration lookupConfiguration, PrimitiveProcessingMethod preferredProcessingMethod)
		: this(new Vector2SequenceProcessor(definition, sequenceConfiguration), new Vector2LookupProcessor(definition, lookupConfiguration), preferredProcessingMethod)
		{ }

		public Vector2Processor(Vector2SequenceProcessor sequenceProcessor, Vector2LookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		: base(sequenceProcessor, lookupProcessor, preferredProcessingMethod)
		{ }
	}
}