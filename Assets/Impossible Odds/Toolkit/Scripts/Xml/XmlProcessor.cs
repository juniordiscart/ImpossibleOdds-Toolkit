namespace ImpossibleOdds.Xml
{
	using System;
	using System.IO;
	using System.Reflection;
	using System.Text;
	using System.Xml;
	using System.Xml.Linq;
	using ImpossibleOdds.Serialization;

	public static class XmlProcessor
	{
		private static XmlOptions defaultOptions = null;

		static XmlProcessor()
		{
			defaultOptions = new XmlOptions();
			defaultOptions.SerializationDefinition = new XmlSerializationDefinition();
			defaultOptions.Encoding = Encoding.UTF8;
			defaultOptions.CompactOutput = true;
		}

		/// <summary>
		/// Serialize the object to an XML-string.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <returns>An XML representation of the given object.</returns>
		public static string Serialize(object obj)
		{
			StringWriter resultStore = new StringWriter();
			Serialize(obj, defaultOptions, resultStore);
			return resultStore.ToString();
		}

		/// <summary>
		/// Serialize the object to an XML string and store the result directly in the result store.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <param name="resultStore">Write cache for the XML result.</param>
		public static void Serialize(object obj, StringBuilder resultStore)
		{
			resultStore.ThrowIfNull(nameof(resultStore));
			Serialize(obj, defaultOptions, new StringWriter(resultStore));
		}

		/// <summary>
		/// Serialize the object to an XML string and store the result directly in the result store.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <param name="resultStore">Write cache for the XML result.</param>
		public static void Serialize(object obj, TextWriter resultStore)
		{
			resultStore.ThrowIfNull(nameof(resultStore));
			Serialize(obj, defaultOptions, resultStore);
		}

		/// <summary>
		/// Serializes the object to an XML string with custom formatting/processing settings.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <param name="options">Options to modify the string output behaviour.</param>
		/// <returns>An XML representation of the given object.</returns>
		public static string Serialize(object obj, XmlOptions options)
		{
			options.ThrowIfNull(nameof(options));

			StringWriter resultStore = new StringWriter();
			Serialize(obj, options, resultStore);
			return resultStore.ToString();
		}

		/// <summary>
		/// Serializes the object to an XML string with custom formatting/processing settings
		/// and store the result directly in the result store.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <param name="options">Options to modify the string output behaviour.</param>
		/// <param name="resultStore">Write cache for the XML result.</param>
		public static void Serialize(object obj, XmlOptions options, StringBuilder resultStore)
		{
			options.ThrowIfNull(nameof(options));
			resultStore.ThrowIfNull(nameof(resultStore));

			Serialize(obj, options, new StringWriter(resultStore));
		}

		/// <summary>
		/// Serializes the object to an XML string with custom formatting/processing settings
		/// and store the result directly in the result store.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <param name="options">Options to modify the string output behaviour.</param>
		/// <param name="resultStore">Write cache for the XML result.</param>
		public static void Serialize(object obj, XmlOptions options, TextWriter resultStore)
		{
			options.ThrowIfNull(nameof(options));
			resultStore.ThrowIfNull(nameof(resultStore));

			XmlSerializationDefinition definition = (options.SerializationDefinition != null) ? options.SerializationDefinition : defaultOptions.SerializationDefinition;
			XDocument document = ToXml(obj, definition);

			XmlWriterSettings writerSettings = new XmlWriterSettings();
			writerSettings.Indent = !options.CompactOutput;
			writerSettings.OmitXmlDeclaration = options.HideHeader;
			writerSettings.Encoding = options.Encoding;

			using (XmlWriter writer = XmlWriter.Create(resultStore, writerSettings))
			{
				document.Save(writer);
			}
		}

		/// <summary>
		/// Deserialize the XML string to an XDocument value which can be used to further process to a custom object.
		/// </summary>
		/// <param name="xmlStr">XML representation of an object.</param>
		/// <returns>An XDocument representing the XML values found in the XML string.</returns>
		public static XDocument Deserialize(string xmlStr)
		{
			return Deserialize(new StringReader(xmlStr));
		}

		/// <summary>
		/// Deserialize the XML string to an XDocument value which can be used to to further process to a custom object.
		/// </summary>
		/// <param name="reader">A reader that will read an XML string.</param>
		/// <returns>An XDocument representing the XML values found in the XML string.</returns>
		public static XDocument Deserialize(TextReader reader)
		{
			using (XmlReader xmlReader = XmlReader.Create(reader))
			{
				XDocument document = XDocument.Load(xmlReader);
				return document;
			}
		}

		/// <summary>
		/// Deserialize the XML string to an instance of the target type.
		/// </summary>
		/// <param name="xmlStr">XML representation of an object.</param>
		/// <typeparam name="TTarget">Target type of the instance.</typeparam>
		/// <returns>Instance of the target type with the values of the XML values applied.</returns>
		public static TTarget Deserialize<TTarget>(string xmlStr)
		{
			return (TTarget)Deserialize(typeof(TTarget), xmlStr);
		}

		/// <summary>
		/// Deserialize the XML values to be read from the text reader to an instance of the target type.
		/// </summary>
		/// <param name="reader">Text reader from which to read the XML values.</param>
		/// <typeparam name="TTarget">Target type of the instance.</typeparam>
		/// <returns>Instance of the target type with the values of the XML values applied.</returns>
		public static TTarget Deserialize<TTarget>(TextReader reader)
		{
			reader.ThrowIfNull(nameof(reader));

			return (TTarget)Deserialize(typeof(TTarget), reader);
		}

		/// <summary>
		/// Deserialize the XML values to be read from the XML document to an instance of the target type.
		/// </summary>
		/// <param name="document">XML document from which to read the XML values.</param>
		/// <typeparam name="TTarget">Target type of the instance.</typeparam>
		/// <returns>Instance of the target type with the values of the XML values applied.</returns>
		public static TTarget Deserialize<TTarget>(XDocument document)
		{
			document.ThrowIfNull(nameof(document));

			return (TTarget)Deserialize(typeof(TTarget), document);
		}

		/// <summary>
		/// Deserialize the XML string to an instance of the target type.
		/// </summary>
		/// <param name="targetType">Target type of the instance.</param>
		/// <param name="xmlStr">XML representation of an object.</param>
		/// <returns>Instance of the target type with the values of the XML values applied.</returns>
		public static object Deserialize(Type targetType, string xmlStr)
		{
			return Deserialize(targetType, new StringReader(xmlStr));
		}

		/// <summary>
		/// Deserialize the XML values to be read from the text reader to an instance of the target type.
		/// </summary>
		/// <param name="targetType">Target type of the instance.</param>
		/// <param name="reader">Text reader from which to read the XML values.</param>
		/// <returns>Instance of the target type with the values of the XML values applied.</returns>
		public static object Deserialize(Type targetType, TextReader reader)
		{
			using (XmlReader xmlReader = XmlReader.Create(reader))
			{
				XDocument document = XDocument.Load(xmlReader);
				return Deserialize(targetType, document);
			}
		}

		/// <summary>
		/// Deserialize the XML values to be read from the XML document to an instance of the target type.
		/// </summary>
		/// <param name="targetType">Target type of the instance.</param>
		/// <param name="document">XML document from which to read the XML values.</param>
		/// <returns>Instance of the target type with the values of the XML values applied.</returns>
		public static object Deserialize(Type targetType, XDocument document)
		{
			targetType.ThrowIfNull(nameof(targetType));
			document.ThrowIfNull(nameof(document));

			return FromXml(targetType, document, defaultOptions.SerializationDefinition);
		}

		/// <summary>
		/// Deserialize the XML string and apply the values to an existing target object.
		/// </summary>
		/// <param name="target">The target object onto which the XML values should be applied.</param>
		/// <param name="xmlStr">XML representation of an object.</param>
		public static void Deserialize(object target, string xmlStr)
		{
			Deserialize(target, new StringReader(xmlStr));
		}

		/// <summary>
		/// Deserialize the XML values to be read from the text reader to an existing target object.
		/// </summary>
		/// <param name="target">The target object onto which the XML values should be applied.</param>
		/// <param name="reader">Text reader from which to read the XML values.</param>
		public static void Deserialize(object target, TextReader reader)
		{
			target.ThrowIfNull(nameof(target));
			reader.ThrowIfNull(nameof(reader));

			using (XmlReader xmlReader = XmlReader.Create(reader))
			{
				XDocument document = XDocument.Load(xmlReader);
				Deserialize(target, document);
			}
		}

		/// <summary>
		/// Deserialize the XML values to be read from the XML document to an existing target object.
		/// </summary>
		/// <param name="target">The target object onto which the XML values should be applied.</param>
		/// <param name="document">XML document from which to read the XML values.</param>
		public static void Deserialize(object target, XDocument document)
		{
			target.ThrowIfNull(nameof(target));
			document.ThrowIfNull(nameof(document));

			FromXml(target, document, defaultOptions.SerializationDefinition);
		}

		private static XDocument ToXml(object objectToSerialize, XmlSerializationDefinition definition)
		{
			definition.ThrowIfNull(nameof(objectToSerialize));

			// If it's already a document, then don't bother.
			if ((objectToSerialize != null) && objectToSerialize is XDocument)
			{
				return objectToSerialize as XDocument;
			}

			// Create the document and add a declaration, if any.
			XDocument document = new XDocument();

			// Create the root element, if any.
			if (objectToSerialize != null)
			{
				// The type should define an XML root attribute.
				Type objectType = objectToSerialize.GetType();
				XmlObjectAttribute rootInfo = objectType.GetCustomAttribute<XmlObjectAttribute>(false);
				if (rootInfo == null)
				{
					throw new XmlException("The object to serialize of type {0} has not defined an {1} attribute.", objectType.Name, typeof(XmlObjectAttribute).Name);
				}

				// Serialize the object and define the name of the root element.
				XElement rootElement = Serializer.Serialize<XElement>(objectToSerialize, definition);
				if (rootElement != null)
				{
					rootElement.Name = !string.IsNullOrEmpty(rootInfo.RootName) ? rootInfo.RootName : objectType.Name;
					rootElement.Add(new XAttribute(XNamespace.Xmlns + XmlSerializationDefinition.XmlSchemaPrefix, XmlSerializationDefinition.XmlSchemaURL));
				}
				document.Add(rootElement);
			}

			return document;
		}

		private static object FromXml(Type targetType, XDocument document, XmlSerializationDefinition definition)
		{
			targetType.ThrowIfNull(nameof(targetType));
			document.ThrowIfNull(nameof(document));
			definition.ThrowIfNull(nameof(definition));

			// If the root element is null, then don't bother and return the default type.
			if (document.Root == null)
			{
				return SerializationUtilities.GetDefaultValue(targetType);
			}

			return Serializer.Deserialize(targetType, document.Root, definition);
		}

		private static void FromXml(object targetObject, XDocument document, XmlSerializationDefinition definition)
		{
			targetObject.ThrowIfNull(nameof(targetObject));
			document.ThrowIfNull(nameof(document));
			definition.ThrowIfNull(nameof(definition));

			if (document.Root != null)
			{
				Serializer.Deserialize(targetObject, document.Root, definition);
			}
		}
	}
}
