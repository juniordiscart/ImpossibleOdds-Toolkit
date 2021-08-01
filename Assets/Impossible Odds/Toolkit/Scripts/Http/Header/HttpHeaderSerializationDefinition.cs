namespace ImpossibleOdds.Http
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Serialization.Processors;

	/// <summary>
	/// Serialization definition for the header of HTTP requests.
	/// </summary>
	public class HttpHeaderSerializationDefinition :
	ILookupSerializationDefinition,
	IEnumAliasSupport<HttpEnumStringAttribute, HttpEnumAliasAttribute>
	{
		private IFormatProvider formatProvider = CultureInfo.InvariantCulture;
		private List<IProcessor> processors = null;
		private HashSet<Type> supportedTypes = null;

		/// <inheritdoc />
		public IEnumerable<ISerializationProcessor> SerializationProcessors
		{
			get
			{
				foreach (IProcessor processor in processors)
				{
					if (processor is ISerializationProcessor serializationProcessor)
					{
						yield return serializationProcessor;
					}
				}
			}
		}

		/// <inheritdoc />
		public IEnumerable<IDeserializationProcessor> DeserializationProcessors
		{
			get
			{
				foreach (IProcessor processor in processors)
				{
					if (processor is IDeserializationProcessor deserializationProcessor)
					{
						yield return deserializationProcessor;
					}
				}
			}
		}

		/// <inheritdoc />
		public HashSet<Type> SupportedTypes
		{
			get { return supportedTypes; }
		}

		/// <summary>
		/// Not implemented because the header definition does not requires objects to be marked for header parameters.
		/// </summary>
		Type ILookupSerializationDefinition.LookupBasedClassMarkingAttribute
		{
			get { throw new NotImplementedException(); }
		}

		/// <inheritdoc />
		public Type LookupBasedFieldAttribute
		{
			get { return typeof(HttpHeaderFieldAttribute); }
		}

		/// <inheritdoc />
		public Type LookupBasedDataType
		{
			get { return typeof(Dictionary<string, string>); }
		}

		/// <inheritdoc />
		public IFormatProvider FormatProvider
		{
			get { return formatProvider; }
			set { formatProvider = value; }
		}

		/// <inheritdoc />
		public Type EnumAsStringAttributeType
		{
			get { return typeof(HttpEnumStringAttribute); }
		}

		/// <inheritdoc />
		public Type EnumAliasValueAttributeType
		{
			get { return typeof(HttpEnumAliasAttribute); }
		}

		public HttpHeaderSerializationDefinition()
		{
			processors = new List<IProcessor>()
			{
				new ExactMatchProcessor(this),
				new EnumProcessor(this),
				new PrimitiveTypeProcessor(this),
				new DateTimeProcessor(this),
				new StringProcessor(this),
				new LookupProcessor(this),
				new CustomObjectLookupProcessor(this, false)
			};

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
