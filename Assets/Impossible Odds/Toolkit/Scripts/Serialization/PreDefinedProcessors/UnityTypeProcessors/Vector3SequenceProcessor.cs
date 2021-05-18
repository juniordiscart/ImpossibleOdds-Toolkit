namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class Vector3SequenceProcessor : UnityPrimitiveSequenceProcessor<Vector3>
	{
		public Vector3SequenceProcessor(IIndexSerializationDefinition definition)
		: base(definition)
		{ }

		protected override Vector3 Deserialize(IList sequenceData)
		{
			return new Vector3(
				Convert.ToSingle(sequenceData[0]),
				Convert.ToSingle(sequenceData[1]),
				Convert.ToSingle(sequenceData[2]));
		}

		protected override IList Serialize(Vector3 value)
		{
			IList result = Definition.CreateSequenceInstance(3);
			result.Add(Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize(value.z, Definition));
			return result;
		}
	}
}
