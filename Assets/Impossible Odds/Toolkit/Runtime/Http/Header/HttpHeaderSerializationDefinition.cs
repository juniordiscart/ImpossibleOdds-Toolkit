using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Processors;

namespace ImpossibleOdds.Http
{
	/// <summary>
	/// Serialization definition for the header of HTTP requests.
	/// </summary>
	public class HttpHeaderSerializationDefinition : ISerializationDefinition
	{
		private readonly ISerializationProcessor[] serializationProcessors;
		private readonly IDeserializationProcessor[] deserializationProcessors;

		/// <inheritdoc />
		public IEnumerable<ISerializationProcessor> SerializationProcessors => serializationProcessors;

		/// <inheritdoc />
		public IEnumerable<IDeserializationProcessor> DeserializationProcessors => deserializationProcessors;

		/// <inheritdoc />
		public HashSet<Type> SupportedTypes { get; }

		/// <inheritdoc />
		public IFormatProvider FormatProvider { get; set; } = CultureInfo.InvariantCulture;

		public HttpHeaderSerializationDefinition()
		{
			// Basic set of types
			SupportedTypes = new HashSet<Type>()
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

			ILookupSerializationConfiguration lookupConfiguration = new HttpHeaderLookupConfiguration();

			List<IProcessor> processors = new List<IProcessor>()
			{
				new NullValueProcessor(this),
				new ExactMatchProcessor(this),
				new EnumProcessor(this)
				{
					AliasFeature = new EnumAliasFeature<HttpEnumStringAttribute, HttpEnumAliasAttribute>()
				},
				new PrimitiveTypeProcessor(this),
				new DateTimeProcessor(this),
				new VersionProcessor(this),
				new GuidProcessor(this),
				new StringProcessor(this),
				new LookupProcessor(this, lookupConfiguration),
				new CustomObjectLookupProcessor(this, lookupConfiguration, false)
				{
					CallbackFeature = new CallBackFeature<OnHttpHeaderSerializingAttribute, OnHttpHeaderSerializedAttribute, OnHttpHeaderDeserializingAttribute, OnHttpHeaderDeserializedAttribute>(),
					RequiredValueFeature = new RequiredValueFeature<HttpHeaderRequiredAttribute>()
				}
			};

			serializationProcessors = processors.Where(p => p is ISerializationProcessor).Cast<ISerializationProcessor>().ToArray();
			deserializationProcessors = processors.Where(p => p is IDeserializationProcessor).Cast<IDeserializationProcessor>().ToArray();
		}
	}
}