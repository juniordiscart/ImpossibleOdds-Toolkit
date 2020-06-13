namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A (de)serialization processor for the Color type.
	/// </summary>
	public class ColorProcessor : AbstractUnityPrimitiveProcessor<Color>
	{
		public ColorProcessor(IIndexSerializationDefinition definition)
		: base(definition)
		{ }

		public ColorProcessor(ILookupSerializationDefinition definition)
		: base(definition)
		{ }

		protected override Color LookupToValue(IDictionary lookup)
		{
			return new Color(Convert.ToSingle(lookup["r"]), Convert.ToSingle(lookup["g"]), Convert.ToSingle(lookup["b"]), Convert.ToSingle(lookup["a"]));
		}

		protected override Color SequenceToValue(IList sequence)
		{
			if (sequence.Count < 4)
			{
				throw new SerializationException("Not enough elements to transform the sequence to an instance of {0}.", typeof(Color).Name);
			}

			return new Color(Convert.ToSingle(sequence[0]), Convert.ToSingle(sequence[1]), Convert.ToSingle(sequence[2]), Convert.ToSingle(sequence[3]));
		}

		protected override IDictionary ValueToLookup(Color value)
		{
			return new Dictionary<string, float>
			{
				{"r", value.r},
				{"g", value.g},
				{"b", value.b},
				{"a", value.a}
			};
		}

		protected override IList ValueToSequence(Color value)
		{
			return new List<float> { value.r, value.g, value.b, value.a };
		}
	}
}
