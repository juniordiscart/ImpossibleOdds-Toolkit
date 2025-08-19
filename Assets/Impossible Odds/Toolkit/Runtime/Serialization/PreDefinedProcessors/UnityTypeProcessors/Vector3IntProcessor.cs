using System;
using System.Collections;
using UnityEngine;

namespace ImpossibleOdds.Serialization.Processors
{
	public class Vector3IntSequenceProcessor : UnityPrimitiveSequenceProcessor<Vector3Int>
	{
		public Vector3IntSequenceProcessor(ISerializationDefinition definition, ISequenceSerializationConfiguration configuration)
		: base(definition, configuration)
		{ }

		public override int Size => 3;

		protected override Vector3Int Deserialize(IList sequenceData)
		{
			return new Vector3Int(
				Convert.ToInt32(sequenceData[0]),
				Convert.ToInt32(sequenceData[1]),
				Convert.ToInt32(sequenceData[2]));
		}

		protected override IList Serialize(Vector3Int value)
		{
			IList result = Configuration.CreateSequenceInstance(Size);
			result.Add(Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize(value.z, Definition));
			return result;
		}
	}

	public class Vector3IntLookupProcessor : UnityPrimitiveLookupProcessor<Vector3Int>
	{
		private static readonly string[] XYZ = { "x", "y", "z" };

		public Vector3IntLookupProcessor(ISerializationDefinition definition, ILookupSerializationConfiguration configuration)
		: base(definition, configuration)
		{ }

		public override string[] Keys => XYZ;

		protected override Vector3Int Deserialize(IDictionary lookupData)
		{
			return new Vector3Int(
				Convert.ToInt32(lookupData[XYZ[0]]),
				Convert.ToInt32(lookupData[XYZ[1]]),
				Convert.ToInt32(lookupData[XYZ[2]]));
		}

		protected override IDictionary Serialize(Vector3Int value)
		{
			IDictionary result = Configuration.CreateLookupInstance(3);
			result.Add(Serializer.Serialize(XYZ[0], Definition), Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(XYZ[1], Definition), Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize(XYZ[2], Definition), Serializer.Serialize(value.z, Definition));
			return result;
		}
	}

	public class Vector3IntProcessor : UnityPrimitiveSwitchProcessor<Vector3IntSequenceProcessor, Vector3IntLookupProcessor, Vector3Int>
	{
		public Vector3IntProcessor(ISerializationDefinition definition, ISequenceSerializationConfiguration sequenceConfiguration, ILookupSerializationConfiguration lookupConfiguration, PrimitiveProcessingMethod preferredProcessingMethod)
		: this(new Vector3IntSequenceProcessor(definition, sequenceConfiguration), new Vector3IntLookupProcessor(definition, lookupConfiguration), preferredProcessingMethod)
		{ }

		public Vector3IntProcessor(Vector3IntSequenceProcessor sequenceProcessor, Vector3IntLookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		: base(sequenceProcessor, lookupProcessor, preferredProcessingMethod)
		{ }
	}
}