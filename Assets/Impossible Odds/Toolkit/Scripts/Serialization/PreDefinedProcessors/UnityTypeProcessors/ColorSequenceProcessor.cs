namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class ColorSequenceProcessor : UnityPrimitiveSequenceProcessor<Color>
	{
		public ColorSequenceProcessor(IIndexSerializationDefinition definition)
		: base(definition)
		{ }

		protected override IList Serialize(Color value)
		{
			IList result = Definition.CreateSequenceInstance(4);
			result.Add(Serializer.Serialize(value.r, Definition));
			result.Add(Serializer.Serialize(value.g, Definition));
			result.Add(Serializer.Serialize(value.b, Definition));
			result.Add(Serializer.Serialize(value.a, Definition));
			return result;
		}

		protected override Color Deserialize(IList sequenceData)
		{
			return new Color(
				Convert.ToSingle(sequenceData[0]),
				Convert.ToSingle(sequenceData[1]),
				Convert.ToSingle(sequenceData[2]),
				Convert.ToSingle(sequenceData[3]));
		}
	}
}
