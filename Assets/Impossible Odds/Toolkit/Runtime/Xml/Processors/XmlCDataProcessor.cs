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

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			if ((objectToSerialize == null) || (objectToSerialize is string))
			{
				return objectToSerialize;
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

			return Convert.ToBase64String(binaryResult);
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			// If the data is a string, then either assign it directly, or convert it to a binary string.
			if ((dataToDeserialize is string stringData))
			{
				if (typeof(string) == targetType)
				{
					return stringData;
				}

				dataToDeserialize = Convert.FromBase64String(stringData);
			}

			// When the data is binary, then try to reconstruct it.
			if (dataToDeserialize is byte[] binaryData)
			{
				if (typeof(byte[]) == targetType)
				{
					return binaryData;
				}

				object deserializedResult = null;
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

				return deserializedResult;
			}
			else
			{
				throw new XmlException("The CDATA value could not be transformed to a valid instance of type {0} for further processing.", typeof(byte[]).Name);
			}
		}

		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			return true;
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			return
				((dataToDeserialize != null) || !SerializationUtilities.IsNullableType(targetType)) &&
				((dataToDeserialize is string) || (dataToDeserialize is byte[]));
		}
	}
}
