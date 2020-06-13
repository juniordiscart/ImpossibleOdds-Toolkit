namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A (de)serialization processor for the Vector4 type.
	/// </summary>
	public class Vector4Processor : AbstractUnityPrimitiveProcessor<Vector4>
	{
		public Vector4Processor(IIndexSerializationDefinition definition)
		: base(definition)
		{ }

		public Vector4Processor(ILookupSerializationDefinition definition)
		: base(definition)
		{ }

		protected override Vector4 LookupToValue(IDictionary lookup)
		{
			return new Vector4(Convert.ToSingle(lookup["x"]), Convert.ToSingle(lookup["y"]), Convert.ToSingle(lookup["z"]), Convert.ToSingle(lookup["w"]));
		}

		protected override Vector4 SequenceToValue(IList sequence)
		{
			if (sequence.Count < 4)
			{
				throw new SerializationException("Not enough elements to transform the sequence to an instance of {0}.", typeof(Vector4).Name);
			}

			return new Vector4(Convert.ToSingle(sequence[0]), Convert.ToSingle(sequence[1]), Convert.ToSingle(sequence[2]), Convert.ToSingle(sequence[3]));
		}

		protected override IDictionary ValueToLookup(Vector4 value)
		{
			return new Dictionary<string, float>
			{
				{"x", value.x},
				{"y", value.y},
				{"z", value.z},
				{"w", value.w}
			};
		}

		protected override IList ValueToSequence(Vector4 value)
		{
			return new List<float> { value.x, value.y, value.z, value.w };
		}
	}
}
