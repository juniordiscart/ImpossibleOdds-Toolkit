namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class Vector4LookupProcessor : UnityPrimitiveLookupProcessor<Vector4>
	{
		public Vector4LookupProcessor(ILookupSerializationDefinition definition)
		: base(definition)
		{ }

		protected override IDictionary Serialize(Vector4 value)
		{
			IDictionary result = Definition.CreateLookupInstance(4);
			result.Add(Serializer.Serialize("x", Definition), Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize("y", Definition), Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize("z", Definition), Serializer.Serialize(value.z, Definition));
			result.Add(Serializer.Serialize("w", Definition), Serializer.Serialize(value.z, Definition));
			return result;
		}

		protected override Vector4 Deserialize(IDictionary lookupData)
		{
			return new Vector4(
				Convert.ToSingle(lookupData["x"]),
				Convert.ToSingle(lookupData["y"]),
				Convert.ToSingle(lookupData["z"]),
				Convert.ToSingle(lookupData["w"]));
		}
	}
}
