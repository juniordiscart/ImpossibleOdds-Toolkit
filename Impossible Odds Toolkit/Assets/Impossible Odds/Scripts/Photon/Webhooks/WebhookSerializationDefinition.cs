#if IMPOSSIBLE_ODDS_PHOTON

namespace ImpossibleOdds.Photon.Webhooks
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Serialization.Processors;

	using Hashtable = ExitGames.Client.Photon.Hashtable;

	/// <summary>
	/// A default implementation to handle (de)serialization of Photon-based webhook calls.
	/// </summary>
	public class WebhookSerializationDefinition : IndexAndLookupDefinition
	<WebhookArrayAttribute, WebhookIndexAttribute, object[],
	WebhookLookupAttribute, WebhookFieldAttribute, Hashtable>
	{
		public override IEnumerable<ISerializationProcessor> SerializationProcessors
		{
			get { return serializationProcessors; }
		}

		public override IEnumerable<IDeserializationProcessor> DeserializationProcessors
		{
			get { return deserializationProcessors; }
		}

		public override HashSet<Type> SupportedTypes
		{
			get { return supportedTypes; }
		}

		private IEnumerable<ISerializationProcessor> serializationProcessors;
		private IEnumerable<IDeserializationProcessor> deserializationProcessors;
		private HashSet<Type> supportedTypes;

		public WebhookSerializationDefinition()
		{
			List<IProcessor> processors = new List<IProcessor>()
			{
				new ExactMatchProcessor(this),
				new EnumProcessor(this),
				new PrimitiveTypeProcessor(this),
				new DateTimeProcessor(this),
				new LookupProcessor(this),
				new SequenceProcessor(this),
				new CustomObjectSequenceProcessor(this),
				new CustomObjectLookupProcessor(this),
				new StringProcessor(this)
			};

			serializationProcessors = processors.Where(p => p is ISerializationProcessor).Cast<ISerializationProcessor>();
			deserializationProcessors = processors.Where(p => p is IDeserializationProcessor).Cast<IDeserializationProcessor>();

			// Based on the list of types supported by the Photon serializer
			// https://doc.photonengine.com/en-us/realtime/current/reference/webrpc#data_types_conversion
			supportedTypes = new HashSet<Type>()
			{
				typeof(byte),
				typeof(short),
				typeof(int),
				typeof(long),
				typeof(double),
				typeof(bool),
				typeof(string),
				typeof(byte[]),
				typeof(Hashtable)
			};
		}
	}
}

#endif
