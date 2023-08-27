﻿namespace ImpossibleOdds.Http
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Serialization.Processors;

	/// <summary>
	/// Serialization definition for the header of HTTP requests.
	/// </summary>
	public class HttpHeaderSerializationDefinition : ILookupSerializationDefinition
	{
		private IFormatProvider formatProvider = CultureInfo.InvariantCulture;
		private ISerializationProcessor[] serializationProcessors = null;
		private IDeserializationProcessor[] deserializationProcessors = null;
		private HashSet<Type> supportedTypes = null;

		/// <inheritdoc />
		public IEnumerable<ISerializationProcessor> SerializationProcessors
		{
			get => serializationProcessors;
		}

		/// <inheritdoc />
		public IEnumerable<IDeserializationProcessor> DeserializationProcessors
		{
			get => deserializationProcessors;
		}

		/// <inheritdoc />
		public HashSet<Type> SupportedTypes
		{
			get => supportedTypes;
		}

		/// <summary>
		/// Not implemented because the header definition does not requires objects to be marked for header parameters.
		/// </summary>
		Type ILookupSerializationDefinition.LookupBasedClassMarkingAttribute
		{
			get => throw new NotImplementedException();
		}

		/// <inheritdoc />
		public Type LookupBasedFieldAttribute
		{
			get => typeof(HttpHeaderFieldAttribute);
		}

		/// <inheritdoc />
		public Type LookupBasedDataType
		{
			get => typeof(Dictionary<string, string>);
		}

		/// <inheritdoc />
		public IFormatProvider FormatProvider
		{
			get => formatProvider;
			set => formatProvider = value;
		}

		public HttpHeaderSerializationDefinition()
		{
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
				new LookupProcessor(this),
				new CustomObjectLookupProcessor(this, false)
				{
					CallbackFeature = new CallBackFeature<OnHttpHeaderSerializingAttribute, OnHttpHeaderSerializedAttribute, OnHttpHeaderDeserializingAttribute, OnHttpHeaderDeserializedAttribute>(),
					RequiredValueFeature = new RequiredValueFeature<HttpHeaderRequiredAttribute>()
				}
			};

			serializationProcessors = processors.Where(p => p is ISerializationProcessor).Cast<ISerializationProcessor>().ToArray();
			deserializationProcessors = processors.Where(p => p is IDeserializationProcessor).Cast<IDeserializationProcessor>().ToArray();
		}

		/// <inheritdoc />
		public Dictionary<string, string> CreateLookupInstance(int capacity)
		{
			return new Dictionary<string, string>(capacity);
		}

		/// <inheritdoc />
		IDictionary ILookupSerializationDefinition.CreateLookupInstance(int capacity)
		{
			return CreateLookupInstance(capacity);
		}
	}
}