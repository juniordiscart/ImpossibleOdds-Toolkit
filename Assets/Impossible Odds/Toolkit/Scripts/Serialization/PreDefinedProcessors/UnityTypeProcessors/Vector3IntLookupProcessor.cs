namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class Vector3IntLookupProcessor : UnityPrimitiveLookupProcessor<Vector3Int>
	{
		public Vector3IntLookupProcessor(ILookupSerializationDefinition definition)
		: base(definition)
		{ }

		protected override Vector3Int Deserialize(IDictionary lookupData)
		{
			return new Vector3Int(
				Convert.ToInt32(lookupData["x"]),
				Convert.ToInt32(lookupData["y"]),
				Convert.ToInt32(lookupData["z"]));
		}

		protected override IDictionary Serialize(Vector3Int value)
		{
			IDictionary result = Definition.CreateLookupInstance(3);
			result.Add(Serializer.Serialize("x", Definition), Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize("y", Definition), Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize("z", Definition), Serializer.Serialize(value.z, Definition));
			return result;
		}
	}
}
