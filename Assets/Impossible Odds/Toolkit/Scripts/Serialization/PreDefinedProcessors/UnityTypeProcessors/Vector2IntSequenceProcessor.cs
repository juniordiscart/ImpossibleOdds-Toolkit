namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class Vector2IntSequenceProcessor : UnityPrimitiveSequenceProcessor<Vector2Int>
	{
		public Vector2IntSequenceProcessor(IIndexSerializationDefinition definition)
		: base(definition)
		{ }

		protected override Vector2Int Deserialize(IList sequenceData)
		{
			return new Vector2Int(
				Convert.ToInt32(sequenceData[0]),
				Convert.ToInt32(sequenceData[1]));
		}

		protected override IList Serialize(Vector2Int value)
		{
			IList result = Definition.CreateSequenceInstance(2);
			result.Add(Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(value.y, Definition));
			return result;
		}
	}
}
