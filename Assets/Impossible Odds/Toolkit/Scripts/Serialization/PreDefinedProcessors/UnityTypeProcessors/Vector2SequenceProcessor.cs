namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class Vector2SequenceProcessor : UnityPrimitiveSequenceProcessor<Vector2>
	{
		public Vector2SequenceProcessor(IIndexSerializationDefinition definition)
		: base(definition)
		{ }

		protected override Vector2 Deserialize(IList sequenceData)
		{
			return new Vector2(
				Convert.ToSingle(sequenceData[0]),
				Convert.ToSingle(sequenceData[1]));
		}

		protected override IList Serialize(Vector2 value)
		{
			IList result = Definition.CreateSequenceInstance(2);
			result.Add(Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(value.y, Definition));
			return result;
		}
	}
}
