namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A (de)serialization processor for the Color32 type.
	/// </summary>
	public class Color32Processor : AbstractUnityPrimitiveProcessor<Color32>
	{
		public Color32Processor(IIndexSerializationDefinition definition)
		: base(definition)
		{ }

		public Color32Processor(ILookupSerializationDefinition definition)
		: base(definition)
		{ }

		protected override Color32 LookupToValue(IDictionary lookup)
		{
			return new Color32(Convert.ToByte(lookup["r"]), Convert.ToByte(lookup["g"]), Convert.ToByte(lookup["b"]), Convert.ToByte(lookup["a"]));
		}

		protected override Color32 SequenceToValue(IList sequence)
		{
			if (sequence.Count < 4)
			{
				throw new SerializationException("Not enough elements to transform the sequence to an instance of {0}.", typeof(Color32).Name);
			}

			return new Color32(Convert.ToByte(sequence[0]), Convert.ToByte(sequence[1]), Convert.ToByte(sequence[2]), Convert.ToByte(sequence[3]));
		}

		protected override IDictionary ValueToLookup(Color32 value)
		{
			return new Dictionary<string, int>
			{
				{"r", value.r},
				{"g", value.g},
				{"b", value.b},
				{"a", value.a}
			};
		}

		protected override IList ValueToSequence(Color32 value)
		{
			return new List<int> { value.r, value.g, value.b, value.a };
		}
	}
}
