using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Processors;

namespace ImpossibleOdds.Photon.WebRpc
{
	/// <summary>
	/// Serialization definition for parameters in the URL of WebRPC requests.
	/// </summary>
	public class WebRpcUrlSerializationDefinition : ISerializationDefinition
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

		public WebRpcUrlSerializationDefinition()
		{
			ILookupSerializationConfiguration lookupConfiguration = new WebRpcURLLookupConfiguration();

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

			List<IProcessor> processors = new List<IProcessor>()
			{
				new NullValueProcessor(this),
				new ExactMatchProcessor(this),
				new EnumProcessor(this)
				{
					AliasFeature = new EnumAliasFeature<WebRpcEnumStringAttribute, WebRpcEnumAliasAttribute>()
				},
				new PrimitiveTypeProcessor(this),
				new DateTimeProcessor(this),
				new VersionProcessor(this),
				new GuidProcessor(this),
				new StringProcessor(this),
				new LookupProcessor(this, lookupConfiguration),
				new CustomObjectLookupProcessor(this, lookupConfiguration, false)
				{
					CallbackFeature = new CallBackFeature<OnWebRpcURLSerializingAttribute, OnWebRpcURLSerializedAttribute, OnWebRpcURLDeserializingAttribute, OnWebRpcURLDeserializedAttribute>(),
					RequiredValueFeature = new RequiredValueFeature<WebRpcURLRequiredAttribute>()
				}
			};

			serializationProcessors = processors.Where(p => p is ISerializationProcessor).Cast<ISerializationProcessor>().ToArray();
			deserializationProcessors = processors.Where(p => p is IDeserializationProcessor).Cast<IDeserializationProcessor>().ToArray();
		}
	}
}