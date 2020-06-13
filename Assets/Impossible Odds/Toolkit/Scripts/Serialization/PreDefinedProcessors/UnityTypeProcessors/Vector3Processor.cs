namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A (de)serialization processor for the Vector3 type.
	/// </summary>
	public class Vector3Processor : AbstractUnityPrimitiveProcessor<Vector3>
	{
		public Vector3Processor(IIndexSerializationDefinition definition) : base(definition)
		{ }

		public Vector3Processor(ILookupSerializationDefinition definition) : base(definition)
		{ }

		protected override Vector3 LookupToValue(IDictionary lookup)
		{
			return new Vector3(Convert.ToSingle(lookup["x"]), Convert.ToSingle(lookup["y"]), Convert.ToSingle(lookup["z"]));
		}

		protected override Vector3 SequenceToValue(IList sequence)
		{
			if (sequence.Count < 3)
			{
				throw new SerializationException("Not enough elements to transform the sequence to an instance of {0}.", typeof(Vector3).Name);
			}

			return new Vector3(Convert.ToSingle(sequence[0]), Convert.ToSingle(sequence[1]), Convert.ToSingle(sequence[2]));
		}

		protected override IDictionary ValueToLookup(Vector3 value)
		{
			return new Dictionary<string, float>
			{
				{"x", value.x},
				{"y", value.y},
				{"z", value.z}
			};
		}

		protected override IList ValueToSequence(Vector3 value)
		{
			return new List<float> { value.x, value.y, value.z };
		}
	}
}
