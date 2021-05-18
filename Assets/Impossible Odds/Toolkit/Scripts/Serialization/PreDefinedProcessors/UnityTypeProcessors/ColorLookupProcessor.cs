namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class ColorLookupProcessor : UnityPrimitiveLookupProcessor<Color>
	{
		public ColorLookupProcessor(ILookupSerializationDefinition definition)
		: base(definition)
		{ }

		protected override IDictionary Serialize(Color value)
		{
			IDictionary result = Definition.CreateLookupInstance(4);
			result.Add(Serializer.Serialize("r", Definition), Serializer.Serialize(value.r, Definition));
			result.Add(Serializer.Serialize("g", Definition), Serializer.Serialize(value.g, Definition));
			result.Add(Serializer.Serialize("b", Definition), Serializer.Serialize(value.b, Definition));
			result.Add(Serializer.Serialize("a", Definition), Serializer.Serialize(value.a, Definition));
			return result;
		}

		protected override Color Deserialize(IDictionary lookupData)
		{
			return new Color(
				Convert.ToSingle(lookupData["r"]),
				Convert.ToSingle(lookupData["g"]),
				Convert.ToSingle(lookupData["b"]),
				Convert.ToSingle(lookupData["a"]));
		}
	}
}
