namespace ImpossibleOdds.Http
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ImpossibleOdds.DataMapping;
	using ImpossibleOdds.DataMapping.Processors;

	public class HttpBodyMappingDefinition : IndexAndLookupDefinition
	<HttpListMappingAttribute, HttpBodyIndexAttribute, List<object>, HttpObjectMappingAttribute, HttpBodyFieldAttribute, Dictionary<string, object>>
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

		public HttpBodyMappingDefinition()
		{
			List<IMappingProcessor> processors = new List<IMappingProcessor>()
			{
				new ExactMatchMappingProcessor(this),
				new EnumMappingProcessor(this),
				new PrimitiveTypeMappingProcessor(this),
				new DateTimeMappingProcessor(this),
				new StringMappingProcessor(this),
				new LookupMappingProcessor(this),
				new SequenceMappingProcessor(this),
				new CustomObjectSequenceMappingProcessor(this),
				new CustomObjectLookupMappingProcessor(this),
			};

			toProcessors = processors.Where(p => p is IMapToDataStructureProcessor).Cast<IMapToDataStructureProcessor>();
			fromProcessors = processors.Where(p => p is IMapFromDataStructureProcessor).Cast<IMapFromDataStructureProcessor>();

			// Basic set of types
			supportedTypes = new HashSet<Type>()
			{
				typeof(short),
				typeof(ushort),
				typeof(int),
				typeof(uint),
				typeof(long),
				typeof(ulong),
				typeof(double),
				typeof(bool),
				typeof(string)
			};
		}
	}
}
