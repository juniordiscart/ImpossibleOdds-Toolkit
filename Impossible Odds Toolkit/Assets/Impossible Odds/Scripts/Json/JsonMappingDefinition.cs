namespace ImpossibleOdds.Json
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	using ImpossibleOdds.DataMapping;
	using ImpossibleOdds.DataMapping.Processors;

	/// <summary>
	/// A mapping definition base with custom JSON object and array type definitions.
	/// </summary>
	/// <typeparam name="TJsonObject">Custom JSON object type.</typeparam>
	/// <typeparam name="TJsonArray">Custom JSON array type.</typeparam>
	public class JsonMappingDefinition<TJsonObject, TJsonArray> :
	IndexAndLookupDefinition<JsonArrayAttribute, JsonIndexAttribute, TJsonArray, JsonObjectAttribute, JsonFieldAttribute, TJsonObject>,
	ILookupTypeResolve<JsonTypeResolveAttribute>
	where TJsonObject : IDictionary, new()
	where TJsonArray : IList, new()
	{
		public Type JsonObjectType
		{
			get { return typeof(TJsonObject); }
		}

		public Type JsonArrayType
		{
			get { return typeof(TJsonArray); }
		}

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

		public Type TypeResolveAttribute
		{
			get { return typeof(JsonTypeResolveAttribute); }
		}

		private IEnumerable<IMapToDataStructureProcessor> toProcessors;
		private IEnumerable<IMapFromDataStructureProcessor> fromProcessors;
		private HashSet<Type> supportedTypes;

		public JsonMappingDefinition()
		{
			List<IMappingProcessor> processors = new List<IMappingProcessor>()
			{
				new ExactMatchMappingProcessor(this),
				new EnumMappingProcessor(this),
				new PrimitiveTypeMappingProcessor(this),
				new DecimalMappingProcessor(this),
				new DateTimeMappingProcessor(this),
				new StringMappingProcessor(this),
				new LookupMappingProcessor(this),
				new SequenceMappingProcessor(this),
				new CustomObjectSequenceMappingProcessor(this),
				new CustomObjectLookupMappingProcessor(this),
			};

			toProcessors = processors.Where(p => p is IMapToDataStructureProcessor).Cast<IMapToDataStructureProcessor>();
			fromProcessors = processors.Where(p => p is IMapFromDataStructureProcessor).Cast<IMapFromDataStructureProcessor>();

			supportedTypes = new HashSet<Type>()
			{
				typeof(byte),
				typeof(short),
				typeof(int),
				typeof(long),
				typeof(double),
				typeof(bool),
				typeof(string),
				typeof(char)
			};
		}
	}

	/// <summary>
	/// A mapping definition with native JSON object and array type parameters.
	/// </summary>
	public sealed class JsonDefaultMappingDefinition : JsonMappingDefinition<Dictionary<string, object>, List<object>>
	{
		public JsonDefaultMappingDefinition()
		: base()
		{ }
	}

}
