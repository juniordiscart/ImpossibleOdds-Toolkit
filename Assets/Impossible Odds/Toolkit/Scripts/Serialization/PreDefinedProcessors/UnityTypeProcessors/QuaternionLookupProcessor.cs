namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class QuaternionLookupProcessor : UnityPrimitiveLookupProcessor<Quaternion>
	{
		public QuaternionLookupProcessor(ILookupSerializationDefinition definition)
		: base(definition)
		{ }

		protected override IDictionary Serialize(Quaternion value)
		{
			IDictionary result = Definition.CreateLookupInstance(4);
			result.Add(Serializer.Serialize("x", Definition), Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize("y", Definition), Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize("z", Definition), Serializer.Serialize(value.z, Definition));
			result.Add(Serializer.Serialize("w", Definition), Serializer.Serialize(value.w, Definition));
			return result;
		}

		protected override Quaternion Deserialize(IDictionary lookupData)
		{
			return new Quaternion(
				Convert.ToSingle(lookupData["x"]),
				Convert.ToSingle(lookupData["y"]),
				Convert.ToSingle(lookupData["z"]),
				Convert.ToSingle(lookupData["w"]));
		}
	}
}
