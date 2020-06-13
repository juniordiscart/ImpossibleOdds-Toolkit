namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A (de)serialization processor for the Vector3Int type.
	/// </summary>
	public class Vector3IntProcessor : AbstractUnityPrimitiveProcessor<Vector3Int>
	{
		public Vector3IntProcessor(IIndexSerializationDefinition definition)
		: base(definition)
		{ }

		public Vector3IntProcessor(ILookupSerializationDefinition definition)
		: base(definition)
		{ }

		protected override Vector3Int LookupToValue(IDictionary lookup)
		{
			return new Vector3Int(Convert.ToInt32(lookup["x"]), Convert.ToInt32(lookup["y"]), Convert.ToInt32(lookup["z"]));
		}

		protected override Vector3Int SequenceToValue(IList sequence)
		{
			if (sequence.Count < 3)
			{
				throw new SerializationException("Not enough elements to transform the sequence to an instance of {0}.", typeof(Vector3Int).Name);
			}

			return new Vector3Int(Convert.ToInt32(sequence[0]), Convert.ToInt32(sequence[1]), Convert.ToInt32(sequence[1]));
		}

		protected override IDictionary ValueToLookup(Vector3Int value)
		{
			return new Dictionary<string, int>
			{
				{"x", value.x},
				{"y", value.y},
				{"z", value.z}
			};
		}

		protected override IList ValueToSequence(Vector3Int value)
		{
			return new List<int> { value.x, value.y, value.z };
		}
	}
}
