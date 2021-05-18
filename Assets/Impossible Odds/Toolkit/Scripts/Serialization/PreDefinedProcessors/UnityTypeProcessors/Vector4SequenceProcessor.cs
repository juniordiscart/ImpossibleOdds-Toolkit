namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class Vector4SequenceProcessor : UnityPrimitiveSequenceProcessor<Vector4>
	{
		public Vector4SequenceProcessor(IIndexSerializationDefinition definition)
		: base(definition)
		{ }

		protected override Vector4 Deserialize(IList sequenceData)
		{
			return new Vector4(
				Convert.ToSingle(sequenceData[0]),
				Convert.ToSingle(sequenceData[1]),
				Convert.ToSingle(sequenceData[2]),
				Convert.ToSingle(sequenceData[3]));
		}

		protected override IList Serialize(Vector4 value)
		{
			IList result = Definition.CreateSequenceInstance(4);
			result.Add(Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize(value.z, Definition));
			result.Add(Serializer.Serialize(value.w, Definition));
			return result;
		}
	}
}
