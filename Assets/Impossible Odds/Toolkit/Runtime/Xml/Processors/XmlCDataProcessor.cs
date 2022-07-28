namespace ImpossibleOdds.Xml.Processors
{
	using System;
	using System.IO;
	using System.Runtime.Serialization.Formatters.Binary;
	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Serialization.Processors;

	/// <summary>
	/// Processor for (de)serializing CDATA sections in an XML document. It uses a binary formatter in combination with formatting the binary result as a Base64 string.
	/// Note: the use of the BinaryFormatter is discouraged for safety purposes. However, there's no (built-in) alternative for transforming data to a stream of bytes.
	/// </summary>
	public class XmlCDataProcessor : ISerializationProcessor, IDeserializationProcessor
	{
		private readonly ISerializationDefinition definition = null;
		private readonly BinaryFormatter binaryFormatter = null;

		public ISerializationDefinition Definition
		{
			get => definition;
		}

		public XmlCDataProcessor(ISerializationDefinition definition)
		{
			definition.ThrowIfNull(nameof(definition));
			this.definition = definition;

			binaryFormatter = new BinaryFormatter();
		}

		public bool Serialize(object objectToSerialize, out object serializedResult)
		{
			if ((objectToSerialize == null) || (objectToSerialize is string))
			{
				serializedResult = objectToSerialize;
				return true;
			}

			byte[] binaryResult = null;
			if (objectToSerialize is byte[])
			{
				binaryResult = objectToSerialize as byte[];
			}
			else
			{
				using (MemoryStream ms = new MemoryStream())
				{
					lock (binaryFormatter)
					{
						binaryFormatter.Serialize(ms, objectToSerialize);
					}
					binaryResult = ms.ToArray();
				}
			}

			serializedResult = Convert.ToBase64String(binaryResult);
			return true;
		}

		public bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
		{
			targetType.ThrowIfNull(nameof(targetType));

			// When the data is null and it can accept null-values, then don't bother.
			if ((dataToDeserialize == null) && SerializationUtilities.IsNullableType(targetType))
			{
				deserializedResult = dataToDeserialize;
				return true;
			}

			// If the data is a string, then either assign it directly, or convert it to a binary string.
			if ((dataToDeserialize is string stringData))
			{
				if (typeof(string) == targetType)
				{
					deserializedResult = stringData;
					return true;
				}

				dataToDeserialize = Convert.FromBase64String(stringData);
			}

			// When the data is binary, then try to reconstruct it.
			if (dataToDeserialize is byte[] binaryData)
			{
				if (typeof(byte[]) == targetType)
				{
					deserializedResult = binaryData;
					return true;
				}

				using (MemoryStream ms = new MemoryStream(binaryData))
				{
					lock (binaryFormatter)
					{
						deserializedResult = binaryFormatter.Deserialize(ms);
					}
				}

				if (!targetType.IsAssignableFrom(deserializedResult.GetType()))
				{
					throw new XmlException("The transformed result of type {0} could not be assigned to target type {1}.", deserializedResult.GetType().Name, targetType.Name);
				}

				return true;
			}
			else
			{
				throw new XmlException("The CDATA value could not be transformed to a valid instance of type {0} for further processing.", typeof(byte[]).Name);
			}
		}
	}
}
