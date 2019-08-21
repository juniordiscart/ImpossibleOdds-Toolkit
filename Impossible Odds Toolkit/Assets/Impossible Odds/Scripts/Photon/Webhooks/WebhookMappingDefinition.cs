#if IMPOSSIBLE_ODDS_PHOTON

namespace ImpossibleOdds.Photon.Webhooks
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using ImpossibleOdds.DataMapping;
	using ImpossibleOdds.DataMapping.Processors;

	using Hashtable = ExitGames.Client.Photon.Hashtable;

	/// <summary>
	/// A default implementation to handle data mapping of Photon-based webhook calls.
	/// </summary>
	public class WebhookMappingDefinition : IndexAndLookupDefinition
	<WebhookArrayMappingAttribute, WebhookIndexAttribute, object[],
	WebhookLookupMappingAttribute, WebhookFieldAttribute, Hashtable>
	{
		public override IEnumerable<IMapToDataStructureProcessor> ToDataStructureProcessors
		{
			get { return toProcessors; }
		}

		public override IEnumerable<IMapFromDataStructureProcessor> FromDataStructureProcessors
		{
			get { return fromProcessors; }
		}

		public override HashSet<Type> SupportedProcessingTypes
		{
			get { return supportedTypes; }
		}

		private IEnumerable<IMapToDataStructureProcessor> toProcessors;
		private IEnumerable<IMapFromDataStructureProcessor> fromProcessors;
		private HashSet<Type> supportedTypes;

		public WebhookMappingDefinition()
		{
			List<IMappingProcessor> processors = new List<IMappingProcessor>()
			{
				new ExactMatchMappingProcessor(this),
				new EnumMappingProcessor(this),
				new PrimitiveTypeMappingProcessor(this),
				new DateTimeMappingProcessor(this),
				new LookupMappingProcessor(this),
				new SequenceMappingProcessor(this),
				new CustomObjectSequenceMappingProcessor(this),
				new CustomObjectLookupMappingProcessor(this),
				new StringMappingProcessor(this)
			};

			toProcessors = processors.Where(p => p is IMapToDataStructureProcessor).Cast<IMapToDataStructureProcessor>();
			fromProcessors = processors.Where(p => p is IMapFromDataStructureProcessor).Cast<IMapFromDataStructureProcessor>();

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
