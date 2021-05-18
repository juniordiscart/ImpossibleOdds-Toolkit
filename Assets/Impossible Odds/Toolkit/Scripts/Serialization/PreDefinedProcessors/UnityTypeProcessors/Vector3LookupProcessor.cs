namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class Vector3LookupProcessor : UnityPrimitiveLookupProcessor<Vector3>
	{
		public Vector3LookupProcessor(ILookupSerializationDefinition definition)
		: base(definition)
		{ }

		protected override IDictionary Serialize(Vector3 value)
		{
			IDictionary result = Definition.CreateLookupInstance(3);
			result.Add(Serializer.Serialize("x", Definition), Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize("y", Definition), Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize("z", Definition), Serializer.Serialize(value.z, Definition));
			return result;
		}

		protected override Vector3 Deserialize(IDictionary lookupData)
		{
			return new Vector3(
				Convert.ToSingle(lookupData["x"]),
				Convert.ToSingle(lookupData["y"]),
				Convert.ToSingle(lookupData["z"]));
		}
	}
}
