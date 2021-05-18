namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class Vector3IntSequenceProcessor : UnityPrimitiveSequenceProcessor<Vector3Int>
	{
		public Vector3IntSequenceProcessor(IIndexSerializationDefinition definition)
		: base(definition)
		{ }

		protected override Vector3Int Deserialize(IList sequenceData)
		{
			return new Vector3Int(
				Convert.ToInt32(sequenceData[0]),
				Convert.ToInt32(sequenceData[1]),
				Convert.ToInt32(sequenceData[2]));
		}

		protected override IList Serialize(Vector3Int value)
		{
			IList result = Definition.CreateSequenceInstance(3);
			result.Add(Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize(value.z, Definition));
			return result;
		}
	}
}
