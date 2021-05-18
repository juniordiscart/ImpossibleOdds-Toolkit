namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using UnityEngine;

	public class Color32LookupProcessor : UnityPrimitiveLookupProcessor<Color32>
	{
		public Color32LookupProcessor(ILookupSerializationDefinition definition)
		: base(definition)
		{ }

		protected override IDictionary Serialize(Color32 value)
		{
			IDictionary result = Definition.CreateLookupInstance(4);
			result.Add(Serializer.Serialize("r", Definition), Serializer.Serialize(value.r, Definition));
			result.Add(Serializer.Serialize("g", Definition), Serializer.Serialize(value.g, Definition));
			result.Add(Serializer.Serialize("b", Definition), Serializer.Serialize(value.b, Definition));
			result.Add(Serializer.Serialize("a", Definition), Serializer.Serialize(value.a, Definition));

			return result;
		}

		protected override Color32 Deserialize(IDictionary lookupData)
		{
			return new Color32(
				Convert.ToByte(lookupData["r"]),
				Convert.ToByte(lookupData["g"]),
				Convert.ToByte(lookupData["b"]),
				Convert.ToByte(lookupData["a"]));
		}
	}
}
