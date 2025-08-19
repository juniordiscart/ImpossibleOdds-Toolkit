using System;
using System.Collections;
using UnityEngine;

namespace ImpossibleOdds.Serialization.Processors
{
	public class QuaternionSequenceProcessor : UnityPrimitiveSequenceProcessor<Quaternion>
	{
		public QuaternionSequenceProcessor(ISerializationDefinition definition, ISequenceSerializationConfiguration configuration)
		: base(definition, configuration)
		{ }

		public override int Size => 4;

		protected override Quaternion Deserialize(IList sequenceData)
		{
			return new Quaternion(
				Convert.ToSingle(sequenceData[0]),
				Convert.ToSingle(sequenceData[1]),
				Convert.ToSingle(sequenceData[2]),
				Convert.ToSingle(sequenceData[3]));
		}

		protected override IList Serialize(Quaternion value)
		{
			IList result = Configuration.CreateSequenceInstance(Size);
			result.Add(Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize(value.z, Definition));
			result.Add(Serializer.Serialize(value.w, Definition));
			return result;
		}
	}

	public class QuaternionLookupProcessor : UnityPrimitiveLookupProcessor<Quaternion>
	{
		private static readonly string[] XYZW = { "x", "y", "z", "w" };

		public QuaternionLookupProcessor(ISerializationDefinition definition, ILookupSerializationConfiguration configuration)
		: base(definition, configuration)
		{ }

		public override string[] Keys => XYZW;

		protected override IDictionary Serialize(Quaternion value)
		{
			IDictionary result = Configuration.CreateLookupInstance(4);
			result.Add(Serializer.Serialize(XYZW[0], Definition), Serializer.Serialize(value.x, Definition));
			result.Add(Serializer.Serialize(XYZW[1], Definition), Serializer.Serialize(value.y, Definition));
			result.Add(Serializer.Serialize(XYZW[2], Definition), Serializer.Serialize(value.z, Definition));
			result.Add(Serializer.Serialize(XYZW[3], Definition), Serializer.Serialize(value.w, Definition));
			return result;
		}

		protected override Quaternion Deserialize(IDictionary lookupData)
		{
			return new Quaternion(
				Convert.ToSingle(lookupData[XYZW[0]]),
				Convert.ToSingle(lookupData[XYZW[1]]),
				Convert.ToSingle(lookupData[XYZW[2]]),
				Convert.ToSingle(lookupData[XYZW[3]]));
		}
	}

	public class QuaternionProcessor : UnityPrimitiveSwitchProcessor<QuaternionSequenceProcessor, QuaternionLookupProcessor, Quaternion>
	{
		public QuaternionProcessor(ISerializationDefinition definition, ISequenceSerializationConfiguration sequenceConfiguration, ILookupSerializationConfiguration lookupConfiguration, PrimitiveProcessingMethod preferredProcessingMethod)
		: this(new QuaternionSequenceProcessor(definition, sequenceConfiguration), new QuaternionLookupProcessor(definition, lookupConfiguration), preferredProcessingMethod)
		{ }

		public QuaternionProcessor(QuaternionSequenceProcessor sequenceProcessor, QuaternionLookupProcessor lookupProcessor, PrimitiveProcessingMethod preferredProcessingMethod)
		: base(sequenceProcessor, lookupProcessor, preferredProcessingMethod)
		{ }
	}
}