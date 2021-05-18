namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class Vector2IntLookupProcessor : UnityPrimitiveLookupProcessor<Vector2Int>
	{
		public Vector2IntLookupProcessor(ILookupSerializationDefinition definition)
		: base(definition)
		{ }

		protected override Vector2Int Deserialize(IDictionary lookupData)
		{
			return new Vector2Int(
				Convert.ToInt32(lookupData["x"]),
				Convert.ToInt32(lookupData["y"]));
		}

		protected override IDictionary Serialize(Vector2Int value)
		{
			IDictionary result = Definition.CreateLookupInstance(2);
			result.Add(Serializer.Serialize("x", Definition), Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize("y", Definition), Serializer.Serialize(value.y, Definition));
			return result;
		}
	}
}
