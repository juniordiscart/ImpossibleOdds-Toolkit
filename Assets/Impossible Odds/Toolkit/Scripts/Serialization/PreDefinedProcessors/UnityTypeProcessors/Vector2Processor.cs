namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A (de)serialization processor for the Vector2 type.
	/// </summary>
	public class Vector2Processor : AbstractUnityPrimitiveProcessor<Vector2>
	{
		public Vector2Processor(IIndexSerializationDefinition definition) : base(definition)
		{ }

		public Vector2Processor(ILookupSerializationDefinition definition) : base(definition)
		{ }

		protected override Vector2 LookupToValue(IDictionary lookup)
		{
			return new Vector2(Convert.ToSingle(lookup["x"]), Convert.ToSingle(lookup["y"]));
		}

		protected override Vector2 SequenceToValue(IList sequence)
		{
			if (sequence.Count < 2)
			{
				throw new SerializationException("Not enough elements to transform the sequence to an instance of {0}.", typeof(Vector2).Name);
			}

			return new Vector2(Convert.ToSingle(sequence[0]), Convert.ToSingle(sequence[1]));
		}

		protected override IDictionary ValueToLookup(Vector2 value)
		{
			return new Dictionary<string, float>
			{
				{"x", value.x},
				{"y", value.y}
			};
		}

		protected override IList ValueToSequence(Vector2 value)
		{
			return new List<float> { value.x, value.y };
		}
	}
}
