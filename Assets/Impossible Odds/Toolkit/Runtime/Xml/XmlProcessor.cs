namespace ImpossibleOdds.Xml
{
	using System;
	using System.IO;
	using System.Text;
	using System.Threading.Tasks;
	using System.Xml;
	using System.Xml.Linq;
	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Serialization.Caching;

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
		/// <param name="options">Options to modify the string output behaviour.</param>
		/// <returns>An XML representation of the given object.</returns>
		public static string Serialize(object obj, XmlOptions options = null)
		{
			using (StringWriter resultStore = new StringWriter())
			{
				Serialize(obj, resultStore, options);
				return resultStore.ToString();
			}
		}

		/// <summary>
		/// Serialize the object to an XML string and write the result to the result store.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <param name="resultStore">Write cache for the XML result.</param>
		/// <param name="options">Options to modify the string output behaviour.</param>
		public static void Serialize(object obj, StringBuilder resultStore, XmlOptions options = null)
		{
			resultStore.ThrowIfNull(nameof(resultStore));

			using (StringWriter writer = new StringWriter(resultStore))
			{
				Serialize(obj, writer, options);
			}
		}

		/// <summary>
		/// Serialize the object to an XML string and write the result to the text writer.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <param name="writer">Text writer for the XML result.</param>
		/// <param name="options">Options to modify the string output behaviour.</param>
		public static void Serialize(object obj, TextWriter writer, XmlOptions options = null)
		{
			writer.ThrowIfNull(nameof(writer));

			if (options == null)
			{
				options = defaultOptions;
			}

			XDocument document = ToXml(obj, (options.SerializationDefinition != null) ? options.SerializationDefinition : defaultOptions.SerializationDefinition);

			XmlWriterSettings writerSettings = new XmlWriterSettings();
			writerSettings.Indent = !options.CompactOutput;
			writerSettings.OmitXmlDeclaration = options.HideHeader;
			writerSettings.Encoding = options.Encoding;

			using (XmlWriter xmlWriter = XmlWriter.Create(writer, writerSettings))
			{
				document.Save(xmlWriter);
			}
		}

		/// <summary>
		/// Obsolete. Use Serialize(object, StringBuilder, XmlOptions) instead.
		/// </summary>
		[Obsolete("Use Serialize(object, StringBuilder, XmlOptions) instead.")]
		public static void Serialize(object obj, XmlOptions options, StringBuilder resultStore)
		{
			Serialize(obj, resultStore, options);
		}

		/// <summary>
		/// Obsolete. Use Serialize(object, TextWriter, XmlOptions) instead.
		/// </summary>
		[Obsolete("Use Serialize(object, TextWriter, XmlOptions) instead.")]
		public static void Serialize(object obj, XmlOptions options, TextWriter resultStore)
		{
			Serialize(obj, resultStore, options);
		}

		/// <summary>
		/// Deserialize the XML string to an XDocument value which can be used to further process to a custom object.
		/// </summary>
		/// <param name="xmlStr">XML representation of an object.</param>
		/// <returns>An XDocument representing the XML values found in the XML string.</returns>
		public static XDocument Deserialize(string xmlStr, XmlOptions options = null)
		{
			return Deserialize(new StringReader(xmlStr), options);
		}

		/// <summary>
		/// Deserialize the XML string to an XDocument value which can be used to to further process to a custom object.
		/// </summary>
		/// <param name="reader">A reader that will read an XML string.</param>
		/// <returns>An XDocument representing the XML values found in the XML string.</returns>
		public static XDocument Deserialize(TextReader reader, XmlOptions options = null)
		{
			XmlReader xmlReader = (options?.ReaderSettings != null) ? XmlReader.Create(reader, options.ReaderSettings) : XmlReader.Create(reader);
			using (xmlReader)
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
		public static TTarget Deserialize<TTarget>(string xmlStr, XmlOptions options = null)
		{
			return (TTarget)Deserialize(typeof(TTarget), xmlStr, options);
		}

		/// <summary>
		/// Deserialize the XML values to be read from the text reader to an instance of the target type.
		/// </summary>
		/// <param name="reader">Text reader from which to read the XML values.</param>
		/// <typeparam name="TTarget">Target type of the instance.</typeparam>
		/// <returns>Instance of the target type with the values of the XML values applied.</returns>
		public static TTarget Deserialize<TTarget>(TextReader reader, XmlOptions options = null)
		{
			reader.ThrowIfNull(nameof(reader));
			return (TTarget)Deserialize(typeof(TTarget), reader, options);
		}

		/// <summary>
		/// Deserialize the XML values to be read from the XML document to an instance of the target type.
		/// </summary>
		/// <param name="document">XML document from which to read the XML values.</param>
		/// <typeparam name="TTarget">Target type of the instance.</typeparam>
		/// <returns>Instance of the target type with the values of the XML values applied.</returns>
		public static TTarget Deserialize<TTarget>(XDocument document, XmlOptions options = null)
		{
			document.ThrowIfNull(nameof(document));
			return (TTarget)Deserialize(typeof(TTarget), document, options);
		}

		/// <summary>
		/// Deserialize the XML string to an instance of the target type.
		/// </summary>
		/// <param name="targetType">Target type of the instance.</param>
		/// <param name="xmlStr">XML representation of an object.</param>
		/// <returns>Instance of the target type with the values of the XML values applied.</returns>
		public static object Deserialize(Type targetType, string xmlStr, XmlOptions options = null)
		{
			return Deserialize(targetType, new StringReader(xmlStr), options);
		}

		/// <summary>
		/// Deserialize the XML values to be read from the text reader to an instance of the target type.
		/// </summary>
		/// <param name="targetType">Target type of the instance.</param>
		/// <param name="reader">Text reader from which to read the XML values.</param>
		/// <returns>Instance of the target type with the values of the XML values applied.</returns>
		public static object Deserialize(Type targetType, TextReader reader, XmlOptions options = null)
		{
			XmlReader xmlReader =
				(options?.ReaderSettings != null) ?
				XmlReader.Create(reader, options.ReaderSettings) :
				XmlReader.Create(reader);

			using (xmlReader)
			{
				return Deserialize(targetType, XDocument.Load(xmlReader), options);
			}
		}

		/// <summary>
		/// Deserialize the XML values to be read from the XML document to an instance of the target type.
		/// </summary>
		/// <param name="targetType">Target type of the instance.</param>
		/// <param name="document">XML document from which to read the XML values.</param>
		/// <returns>Instance of the target type with the values of the XML values applied.</returns>
		public static object Deserialize(Type targetType, XDocument document, XmlOptions options = null)
		{
			targetType.ThrowIfNull(nameof(targetType));
			document.ThrowIfNull(nameof(document));

			XmlSerializationDefinition sd = (options?.SerializationDefinition != null) ? options.SerializationDefinition : defaultOptions.SerializationDefinition;
			return FromXml(targetType, document, sd);
		}

		/// <summary>
		/// Deserialize the XML string and apply the values to an existing target object.
		/// </summary>
		/// <param name="target">The target object onto which the XML values should be applied.</param>
		/// <param name="xmlStr">XML representation of an object.</param>
		public static void Deserialize(object target, string xmlStr, XmlOptions options = null)
		{
			Deserialize(target, new StringReader(xmlStr), options);
		}

		/// <summary>
		/// Deserialize the XML values to be read from the text reader to an existing target object.
		/// </summary>
		/// <param name="target">The target object onto which the XML values should be applied.</param>
		/// <param name="reader">Text reader from which to read the XML values.</param>
		public static void Deserialize(object target, TextReader reader, XmlOptions options = null)
		{
			target.ThrowIfNull(nameof(target));
			reader.ThrowIfNull(nameof(reader));

			XmlReader xmlReader =
				(options?.ReaderSettings != null) ?
				XmlReader.Create(reader, options.ReaderSettings) :
				XmlReader.Create(reader);

			XmlSerializationDefinition sd =
				(options?.SerializationDefinition != null) ?
				options.SerializationDefinition :
				defaultOptions.SerializationDefinition;

			ISerializationReflectionMap reflectionMap = SerializationUtilities.GetTypeMap(target.GetType());

			using (xmlReader)
			{
				XDocument document = FromXml(new DeserializationState(sd, xmlReader), DataFilterMode.Filter, reflectionMap);
				FromXml(target, document, sd);
			}
		}

		/// <summary>
		/// Deserialize the XML values to be read from the XML document to an existing target object.
		/// </summary>
		/// <param name="target">The target object onto which the XML values should be applied.</param>
		/// <param name="document">XML document from which to read the XML values.</param>
		public static void Deserialize(object target, XDocument document, XmlOptions options = null)
		{
			target.ThrowIfNull(nameof(target));
			document.ThrowIfNull(nameof(document));

			XmlSerializationDefinition sd =
				(options?.SerializationDefinition != null) ?
				options.SerializationDefinition :
				defaultOptions.SerializationDefinition;

			FromXml(target, document, sd);
		}

		#region Async
		/// <summary>
		/// Serialize the object async to an XML-string.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <param name="options">Options to modify the string output behaviour.</param>
		/// <returns>An XML representation of the given object.</returns>
		public static async Task<string> SerializeAsync(object obj, XmlOptions options = null)
		{
			Task<string> asyncResult = Task.Run(() => Serialize(obj, options));
			await asyncResult;
			return asyncResult.Result;
		}

		/// <summary>
		/// Serialize the object async to an XML string and write the result to the result store.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <param name="resultStore">Write cache for the XML result.</param>
		/// <param name="options">Options to modify the string output behaviour.</param>
		public static async Task SerializeAsync(object obj, StringBuilder resultStore, XmlOptions options = null)
		{
			await Task.Run(() => Serialize(obj, resultStore, options));
		}

		/// <summary>
		/// Serialize the object async to an XML string and write the result to the text writer.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <param name="writer">Text writer for the XML result.</param>
		/// <param name="options">Options to modify the string output behaviour.</param>
		public static async Task SerializeAsync(object obj, TextWriter writer, XmlOptions options = null)
		{
			await Task.Run(() => Serialize(obj, writer, options));
		}

		/// <summary>
		/// Deserialize the XML string async to an XDocument value which can be used to further process to a custom object.
		/// </summary>
		/// <param name="xmlStr">XML representation of an object.</param>
		/// <returns>An XDocument representing the XML values found in the XML string.</returns>
		public static async Task<XDocument> DeserializeAsync(string xmlStr, XmlOptions options = null)
		{
			Task<XDocument> asyncResult = Task.Run(() => Deserialize(xmlStr, options));
			await asyncResult;
			return asyncResult.Result;
		}

		/// <summary>
		/// Deserialize the XML string async to an XDocument value which can be used to to further process to a custom object.
		/// </summary>
		/// <param name="reader">A reader that will read an XML string.</param>
		/// <returns>An XDocument representing the XML values found in the XML string.</returns>
		public static async Task<XDocument> DeserializeAsync(TextReader reader, XmlOptions options = null)
		{
			Task<XDocument> asyncResult = Task.Run(() => Deserialize(reader, options));
			await asyncResult;
			return asyncResult.Result;
		}

		/// <summary>
		/// Deserialize the XML string async to an instance of the target type.
		/// </summary>
		/// <param name="xmlStr">XML representation of an object.</param>
		/// <typeparam name="TTarget">Target type of the instance.</typeparam>
		/// <returns>Instance of the target type with the values of the XML values applied.</returns>
		public static async Task<TTarget> DeserializeAsync<TTarget>(string xmlStr, XmlOptions options = null)
		{
			Task<TTarget> asyncResult = Task.Run(() => Deserialize<TTarget>(xmlStr, options));
			await asyncResult;
			return asyncResult.Result;
		}

		/// <summary>
		/// Deserialize the XML values async to be read from the text reader to an instance of the target type.
		/// </summary>
		/// <param name="reader">Text reader from which to read the XML values.</param>
		/// <typeparam name="TTarget">Target type of the instance.</typeparam>
		/// <returns>Instance of the target type with the values of the XML values applied.</returns>
		public static async Task<TTarget> DeserializeAsync<TTarget>(TextReader reader, XmlOptions options = null)
		{
			Task<TTarget> asyncResult = Task.Run(() => Deserialize<TTarget>(reader, options));
			await asyncResult;
			return asyncResult.Result;
		}

		/// <summary>
		/// Deserialize the XML values async to be read from the XML document to an instance of the target type.
		/// </summary>
		/// <param name="document">XML document from which to read the XML values.</param>
		/// <typeparam name="TTarget">Target type of the instance.</typeparam>
		/// <returns>Instance of the target type with the values of the XML values applied.</returns>
		public static async Task<TTarget> DeserializeAsync<TTarget>(XDocument document, XmlOptions options = null)
		{
			Task<TTarget> asyncResult = Task.Run(() => Deserialize<TTarget>(document, options));
			await asyncResult;
			return asyncResult.Result;
		}

		/// <summary>
		/// Deserialize the XML string async to an instance of the target type.
		/// </summary>
		/// <param name="targetType">Target type of the instance.</param>
		/// <param name="xmlStr">XML representation of an object.</param>
		/// <returns>Instance of the target type with the values of the XML values applied.</returns>
		public static async Task<object> DeserializeAsync(Type targetType, string xmlStr, XmlOptions options = null)
		{
			Task<object> asyncResult = Task.Run(() => Deserialize(targetType, xmlStr, options));
			await asyncResult;
			return asyncResult.Result;
		}

		/// <summary>
		/// Deserialize the XML values async to be read from the text reader to an instance of the target type.
		/// </summary>
		/// <param name="targetType">Target type of the instance.</param>
		/// <param name="reader">Text reader from which to read the XML values.</param>
		/// <returns>Instance of the target type with the values of the XML values applied.</returns>
		public static async Task<object> DeserializeAsync(Type targetType, TextReader reader, XmlOptions options = null)
		{
			Task<object> asyncResult = Task.Run(() => Deserialize(targetType, reader, options));
			await asyncResult;
			return asyncResult.Result;
		}

		/// <summary>
		/// Deserialize the XML values async to be read from the XML document to an instance of the target type.
		/// </summary>
		/// <param name="targetType">Target type of the instance.</param>
		/// <param name="document">XML document from which to read the XML values.</param>
		/// <returns>Instance of the target type with the values of the XML values applied.</returns>
		public static async Task<object> DeserializeAsync(Type targetType, XDocument document, XmlOptions options = null)
		{
			Task<object> asyncResult = Task.Run(() => Deserialize(targetType, document, options));
			await asyncResult;
			return asyncResult.Result;
		}

		/// <summary>
		/// Deserialize the XML string async and apply the values to an existing target object.
		/// </summary>
		/// <param name="target">The target object onto which the XML values should be applied.</param>
		/// <param name="xmlStr">XML representation of an object.</param>
		public static async Task DeserializeAsync(object target, string xmlStr, XmlOptions options = null)
		{
			await Task.Run(() => Deserialize(target, xmlStr, options));
		}

		/// <summary>
		/// Deserialize the XML values async to be read from the text reader to an existing target object.
		/// </summary>
		/// <param name="target">The target object onto which the XML values should be applied.</param>
		/// <param name="reader">Text reader from which to read the XML values.</param>
		public static async Task DeserializeAsync(object target, TextReader reader, XmlOptions options = null)
		{
			await Task.Run(() => Deserialize(target, reader, options));
		}

		/// <summary>
		/// Deserialize the XML values async to be read from the XML document to an existing target object.
		/// </summary>
		/// <param name="target">The target object onto which the XML values should be applied.</param>
		/// <param name="document">XML document from which to read the XML values.</param>
		public static async Task DeserializeAsync(object target, XDocument document, XmlOptions options = null)
		{
			await Task.Run(() => Deserialize(target, document, options));
		}

		#endregion

		#region To XML
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
				XmlObjectAttribute rootInfo = Attribute.GetCustomAttribute(objectType, typeof(XmlObjectAttribute), false) as XmlObjectAttribute;
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
		#endregion

		#region From XML
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

		private static XDocument FromXml(DeserializationState reader, DataFilterMode filterMode = DataFilterMode.ProcessAll, ISerializationReflectionMap reflectionMap = null)
		{
			reader.ThrowIfNull(nameof(reader));

			XDocument document = new XDocument();

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.XmlDeclaration)
				{
					document.Declaration = ReadDeclaration(reader);
				}
				else
				{
					switch(filterMode)
					{
						case DataFilterMode.ProcessAll:
						{
							if (ReadNext(reader, out XNode childNode))
							{
								document.Add(childNode);
							}
						}
							break;
						case DataFilterMode.Filter:
						{
							if (FilterNext(reader, out XNode childNode, reflectionMap))
							{
								document.Add(childNode);
							}
						}
							break;
						case DataFilterMode.Skip:
							SkipNext(reader);
							break;
					}
				}
			}

			return document;
		}

		private static bool ReadNext(DeserializationState reader, out XNode node)
		{
			switch (reader.NodeType)
			{
				case XmlNodeType.Element:
					node = ReadElement(reader);
					break;
				case XmlNodeType.Text:
				case XmlNodeType.SignificantWhitespace:
				case XmlNodeType.Whitespace:
					node = ReadTextNode(reader);
					break;
				case XmlNodeType.CDATA:
					node = ReadCDataNode(reader);
					break;
				case XmlNodeType.Comment:
					node = ReadComment(reader);
					break;
				case XmlNodeType.DocumentType:
					node = ReadDocumentType(reader);
					break;
				case XmlNodeType.ProcessingInstruction:
					node = ReadProcessingInstruction(reader);
					break;
				case XmlNodeType.EndElement:
					node = null;
					break;
				default:
					Log.Warning("Xml node type {0} is not supported.", reader.NodeType.DisplayName());
					node = null;
					break;
			}

			return node != null;
		}

		private static bool FilterNext(DeserializationState reader, out XNode node, ISerializationReflectionMap reflectionMap)
		{
			// Only elements need to filtered. All other content can be just be read.
			if (reader.NodeType == XmlNodeType.Element)
			{
				node = FilterElement(reader, reflectionMap);
			}
			else
			{
				ReadNext(reader, out node);
			}

			return node != null;
		}

		private static void SkipNext(DeserializationState reader)
		{
			if (reader.NodeType == XmlNodeType.Element)
			{
				SkipElement(reader);	// The elements still need to be read from the reader to advance.
			}
		}

		private static XDeclaration ReadDeclaration(DeserializationState reader)
		{
			if (reader.NodeType != XmlNodeType.XmlDeclaration)
			{
				throw new XmlException("A declaration node is expected. Received: {0}.", reader.NodeType.DisplayName());
			}

			string version = reader.MoveToAttribute("version") ? reader.Value : string.Empty;
			string encoding = reader.MoveToAttribute("encoding") ? reader.Value : string.Empty;
			string standalone = reader.MoveToAttribute("standalone") ? reader.Value : string.Empty;
			reader.MoveToElement();

			return new XDeclaration(version, encoding, standalone);
		}

		private static XElement ReadElement(DeserializationState reader)
		{
			if (reader.NodeType != XmlNodeType.Element)
			{
				throw new XmlException("An element node is expected. Received: {0}.", reader.NodeType.DisplayName());
			}

			XElement element = new XElement(reader.Name);
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					element.Add(ReadAttribute(reader));
				}

				reader.MoveToElement();
			}

			// Return already if it's empty, e.g. <element />
			if (reader.IsEmptyElement)
			{
				return element;
			}

			// Keep reading until encountering the end element.
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					break;
				}

				if (ReadNext(reader, out XNode nextNode))
				{
					element.Add(nextNode);
				}
			}

			return element;
		}

		private static XElement FilterElement(DeserializationState reader, ISerializationReflectionMap reflectionMap)
		{
			if (reader.NodeType != XmlNodeType.Element)
			{
				throw new XmlException("An element node is expected. Received: {0}.", reader.NodeType.DisplayName());
			}

			XElement element = new XElement(reader.Name);
			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					XAttribute attr = FilterAttribute(reader, reflectionMap);
					if (attr != null)
					{
						element.Add(attr);
					}
				}

				reader.MoveToElement();
			}

			// Return already if it's empty, e.g. <element />
			if (reader.IsEmptyElement)
			{
				return element;
			}

			ISerializableMember[] members = reflectionMap.GetSerializableMembers(typeof(XmlElementAttribute));

			// Keep reading until encountering the end element.
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				else if ((reader.NodeType == XmlNodeType.Element) && !KeyExists<XmlElementAttribute>(members, reader.Name))
				{
					SkipNext(reader);
				}
				else
				{
					if (ReadNext(reader, out XNode nextNode))
					{
						element.Add(nextNode);
					}
				}
			}

			return element;
		}

		private static void SkipElement(DeserializationState reader)
		{
			if (reader.NodeType != XmlNodeType.Element)
			{
				throw new XmlException("An element node is expected. Received: {0}.", reader.NodeType.DisplayName());
			}

			// Return already if it's empty, e.g. <element />
			if (reader.IsEmptyElement)
			{
				return;
			}

			// Keep reading until encountering the end element.
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					break;
				}

				SkipNext(reader);
			}
		}

		private static XText ReadTextNode(DeserializationState reader)
		{
			return new XText(reader.Value);
		}

		private static XCData ReadCDataNode(DeserializationState reader)
		{
			return new XCData(reader.Value);
		}

		private static XComment ReadComment(DeserializationState reader)
		{
			return new XComment(reader.Value);
		}

		private static XDocumentType ReadDocumentType(DeserializationState reader)
		{
			return new XDocumentType(
				reader.Name,
				reader.GetAttribute("PUBLIC"),
				reader.GetAttribute("SYSTEM"),
				reader.Value);
		}

		private static XProcessingInstruction ReadProcessingInstruction(DeserializationState reader)
		{
			return new XProcessingInstruction(reader.Name, reader.Value);
		}

		private static XAttribute ReadAttribute(DeserializationState reader)
		{
			return new XAttribute(reader.Name, reader.Value);
		}

		private static XAttribute FilterAttribute(DeserializationState reader, ISerializationReflectionMap reflectionMap)
		{
			ISerializableMember[] members = reflectionMap.GetSerializableMembers(typeof(XmlAttributeAttribute));

			return KeyExists<XmlAttributeAttribute>(members, reader.Name) ? ReadAttribute(reader) : null;
		}

		private static bool KeyExists<TAttribute>(ISerializableMember[] members, string key)
		where TAttribute : ILookupParameter<string>
		{
			foreach (ISerializableMember member in members)
			{
				if ((member.Attribute is TAttribute parameter))
				{
					string memberKey = parameter.Key;

					if (memberKey.IsNullOrEmpty())
					{
						memberKey = member.Member.Name;
					}

					if (string.Equals(key, memberKey))
					{
						return true;
					}
				}
			}

			return false;
		}
		#endregion

	}
}
