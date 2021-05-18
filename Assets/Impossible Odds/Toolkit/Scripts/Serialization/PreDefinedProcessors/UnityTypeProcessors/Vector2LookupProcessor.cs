namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class Vector2LookupProcessor : UnityPrimitiveLookupProcessor<Vector2>
	{
		public Vector2LookupProcessor(ILookupSerializationDefinition definition)
		: base(definition)
		{ }

		protected override IDictionary Serialize(Vector2 value)
		{
			IDictionary result = Definition.CreateLookupInstance(2);
			result.Add(Serializer.Serialize("x", Definition), Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize("y", Definition), Serializer.Serialize(value.y, Definition));
			return result;
		}

		protected override Vector2 Deserialize(IDictionary lookupData)
		{
			return new Vector2(
				Convert.ToSingle(lookupData["x"]),
				Convert.ToSingle(lookupData["y"]));
		}
	}
}
