using System;
using System.Collections;
using UnityEngine;

namespace ImpossibleOdds.Serialization.Processors
{

	public class Vector3SequenceProcessor : UnityPrimitiveSequenceProcessor<Vector3>
	{
		public Vector3SequenceProcessor(ISerializationDefinition definition, ISequenceSerializationConfiguration configuration)
		: base(definition, configuration)
		{ }

		public override int Size => 3;

		protected override Vector3 Deserialize(IList sequenceData)
		{
			return new Vector3(
				Convert.ToSingle(sequenceData[0]),
				Convert.ToSingle(sequenceData[1]),
				Convert.ToSingle(sequenceData[2]));
		}

		protected override IList Serialize(Vector3 value)
		{
			IList result = Configuration.CreateSequenceInstance(Size);
			result.Add(Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize(value.z, Definition));
			return result;
		}
	}

	public class Vector3LookupProcessor : UnityPrimitiveLookupProcessor<Vector3>
	{
		private static readonly string[] XYZ = { "x", "y", "z" };

		public Vector3LookupProcessor(ISerializationDefinition definition, ILookupSerializationConfiguration configuration)
		: base(definition, configuration)
		{ }

		public override string[] Keys => XYZ;

		protected override IDictionary Serialize(Vector3 value)
		{
			IDictionary result = Configuration.CreateLookupInstance(3);
			result.Add(Serializer.Serialize(XYZ[0], Definition), Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(XYZ[1], Definition), Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize(XYZ[2], Definition), Serializer.Serialize(value.z, Definition));
			return result;
		}

		protected override Vector3 Deserialize(IDictionary lookupData)
		{
			return new Vector3(
				Convert.ToSingle(lookupData[XYZ[0]]),
				Convert.ToSingle(lookupData[XYZ[1]]),
				Convert.ToSingle(lookupData[XYZ[2]]));
		}
	}

	public class Vector3Processor : UnityPrimitiveSwitchProcessor<Vector3SequenceProcessor, Vector3LookupProcessor, Vector3>
	{
		public Vector3Processor(ISerializationDefinition definition, ISequenceSerializationConfiguration sequenceConfiguration, ILookupSerializationConfiguration lookupConfiguration, PrimitiveProcessingMethod preferredProcessingMethod)
		: this(new Vector3SequenceProcessor(definition, sequenceConfiguration), new Vector3LookupProcessor(definition, lookupConfiguration), preferredProcessingMethod)
		{ }

		public Vector3Processor(Vector3SequenceProcessor sequenceProcessor, Vector3LookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		: base(sequenceProcessor, lookupProcessor, preferredProcessingMethod)
		{ }
	}
}