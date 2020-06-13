namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A (de)serialization processor for the Vector2Int type.
	/// </summary>
	public class Vector2IntProcessor : AbstractUnityPrimitiveProcessor<Vector2Int>
	{
		public Vector2IntProcessor(IIndexSerializationDefinition definition)
		: base(definition)
		{ }

		public Vector2IntProcessor(ILookupSerializationDefinition definition)
		: base(definition)
		{ }

		protected override Vector2Int LookupToValue(IDictionary lookup)
		{
			return new Vector2Int(Convert.ToInt32(lookup["x"]), Convert.ToInt32(lookup["y"]));
		}

		protected override Vector2Int SequenceToValue(IList sequence)
		{
			if (sequence.Count < 2)
			{
				throw new SerializationException("Not enough elements to transform the sequence to an instance of {0}.", typeof(Vector2Int).Name);
			}

			return new Vector2Int(Convert.ToInt32(sequence[0]), Convert.ToInt32(sequence[1]));
		}

		protected override IDictionary ValueToLookup(Vector2Int value)
		{
			return new Dictionary<string, int>
			{
				{"x", value.x},
				{"y", value.y}
			};
		}

		protected override IList ValueToSequence(Vector2Int value)
		{
			return new List<int> { value.x, value.y };
		}
	}
}
