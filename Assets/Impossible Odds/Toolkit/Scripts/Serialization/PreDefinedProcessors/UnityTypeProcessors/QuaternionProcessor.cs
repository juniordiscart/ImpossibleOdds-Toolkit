namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A (de)serialization processor for the Quaternion type.
	/// </summary>
	public class QuaternionProcessor : AbstractUnityPrimitiveProcessor<Quaternion>
	{
		public QuaternionProcessor(IIndexSerializationDefinition definition)
		: base(definition)
		{ }

		public QuaternionProcessor(ILookupSerializationDefinition definition)
		: base(definition)
		{ }

		protected override Quaternion LookupToValue(IDictionary lookup)
		{
			return new Quaternion(Convert.ToSingle(lookup["x"]), Convert.ToSingle(lookup["y"]), Convert.ToSingle(lookup["z"]), Convert.ToSingle(lookup["w"]));
		}

		protected override Quaternion SequenceToValue(IList sequence)
		{
			if (sequence.Count < 4)
			{
				throw new SerializationException("Not enough elements to transform the sequence to an instance of {0}.", typeof(Quaternion).Name);
			}

			return new Quaternion(Convert.ToSingle(sequence[0]), Convert.ToSingle(sequence[1]), Convert.ToSingle(sequence[2]), Convert.ToSingle(sequence[3]));
		}

		protected override IDictionary ValueToLookup(Quaternion value)
		{
			return new Dictionary<string, float>
			{
				{"x", value.x},
				{"y", value.y},
				{"z", value.z},
				{"w", value.w}
			};
		}

		protected override IList ValueToSequence(Quaternion value)
		{
			return new List<float> { value.x, value.y, value.z, value.w };
		}
	}
}
