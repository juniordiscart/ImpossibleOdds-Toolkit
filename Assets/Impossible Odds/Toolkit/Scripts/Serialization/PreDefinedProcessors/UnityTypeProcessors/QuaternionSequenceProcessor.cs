namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class QuaternionSequenceProcessor : UnityPrimitiveSequenceProcessor<Quaternion>
	{
		public QuaternionSequenceProcessor(IIndexSerializationDefinition definition)
		: base(definition)
		{ }

		protected override Quaternion Deserialize(IList sequenceData)
		{
			return new Quaternion(
				Convert.ToSingle(sequenceData[0]),
				Convert.ToSingle(sequenceData[1]),
				Convert.ToSingle(sequenceData[2]),
				Convert.ToSingle(sequenceData[3]));
		}

		protected override IList Serialize(Quaternion value)
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
