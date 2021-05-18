namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class Color32SequenceProcessor : UnityPrimitiveSequenceProcessor<Color32>
	{
		public Color32SequenceProcessor(IIndexSerializationDefinition definition)
		: base(definition)
		{ }

		protected override IList Serialize(Color32 value)
		{
			IList result = Definition.CreateSequenceInstance(4);
			result.Add(Serializer.Serialize(value.r, Definition));
			result.Add(Serializer.Serialize(value.g, Definition));
			result.Add(Serializer.Serialize(value.b, Definition));
			result.Add(Serializer.Serialize(value.a, Definition));
			return result;
		}

		protected override Color32 Deserialize(IList sequenceData)
		{
			return new Color32(
				Convert.ToByte(sequenceData[0]),
				Convert.ToByte(sequenceData[1]),
				Convert.ToByte(sequenceData[2]),
				Convert.ToByte(sequenceData[3]));
		}
	}
}
