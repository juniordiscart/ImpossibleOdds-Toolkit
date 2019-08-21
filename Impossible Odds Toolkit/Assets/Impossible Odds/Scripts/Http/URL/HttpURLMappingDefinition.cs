namespace ImpossibleOdds.Http
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using ImpossibleOdds.DataMapping;
	using ImpossibleOdds.DataMapping.Processors;

	public class HttpURLMappingDefinition : LookupDefinition
	<HttpObjectMappingAttribute, HttpURLFieldAttribute, Dictionary<string, string>>
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

		public HttpURLMappingDefinition()
		{
			List<IMappingProcessor> processors = new List<IMappingProcessor>()
			{
				new ExactMatchMappingProcessor(this),
				new EnumMappingProcessor(this),
				new PrimitiveTypeMappingProcessor(this),
				new DateTimeMappingProcessor(this),
				new StringMappingProcessor(this),
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
				typeof(float),
				typeof(double),
				typeof(bool),
				typeof(string)
			};
		}
	}
}
