namespace ImpossibleOdds.Json
{
	using System;
	using System.Collections;
	using System.Text;
	using System.Text.RegularExpressions;
	using ImpossibleOdds.Serialization;

	public static class JsonProcessor
	{
		private const int DefaultResultsCacheCapacity = 1024;
		private const string NullStr = "null";
		private const string TrueStr = "true";
		private const string FalseStr = "false";

		private readonly static Regex numericalRegex = new Regex(@"^-?(?:0|[1-9]\d*)(?:\.\d+)?(?:[eE][+-]?\d+)?$");
		private readonly static char[] numericalSymbols = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '+', 'e', 'E', '.' };
		private static JsonSerializationDefinition defaultSerializationDefinition = new JsonSerializationDefinition();

		/// <summary>
		/// Serialize the object to a JSON-string.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <param name="options">Options to modify the string output behaviour.</param>
		/// <returns>A JSON representation of the given object.</returns>
		public static string Serialize(object obj, JsonOptions options = null)
		{
			SerializationSettings serializationSettings = new SerializationSettings();
			if (options != null)
			{
				serializationSettings.ApplyOptions(options);
			}

			ToJson(obj, serializationSettings);
			return serializationSettings.resultStore.ToString();
		}

		/// <summary>
		/// Serialize the object to a JSON string and store the result directly in the result store.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <param name="resultStore">Write cache for the JSON result.</param>
		/// <param name="options">Options to modify the string output behaviour.</param>
		public static void Serialize(object obj, StringBuilder resultStore, JsonOptions options = null)
		{
			resultStore.ThrowIfNull(nameof(resultStore));

			SerializationSettings serializationSettings = new SerializationSettings(resultStore);
			if (options != null)
			{
				serializationSettings.ApplyOptions(options);
			}

			ToJson(obj, serializationSettings);
		}

		/// <summary>
		/// Serialize the object to a JSON string with custom formatting/processing settings
		/// and store the result directly in the result store.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <param name="options">Options to modify the string output behaviour.</param>
		/// <param name="resultStore">Write cache for the JSON result.</param>
		[Obsolete("Use JsonProcessor.Serialize(object obj, StringBuilder resultStore, JsonOptions options = null) instead")]
		public static void Serialize(object obj, JsonOptions options, StringBuilder resultStore)
		{
			options.ThrowIfNull(nameof(options));
			resultStore.ThrowIfNull(nameof(resultStore));
			SerializationSettings serializationSettings = new SerializationSettings(resultStore);
			serializationSettings.ApplyOptions(options);

			ToJson(obj, serializationSettings);
		}

		/// <summary>
		/// Process a JSON string to a data structure.
		/// </summary>
		/// <param name="jsonStr">JSON representation of an object.</param>
		/// <param name="options">Additional options to customize the deserialization behaviour.</param>
		public static object Deserialize(string jsonStr, JsonOptions options = null)
		{
			ISerializationDefinition sd = (options?.SerializationDefinition != null) ? options.SerializationDefinition : defaultSerializationDefinition;
			int index = 0;
			return FromJson(sd, jsonStr, ref index);
		}

		/// <summary>
		/// Process a JSON string value and attempt to deserialize it to an instance of the target type.
		/// </summary>
		/// <param name="jsonStr">JSON representation of an object.</param>
		/// <param name="options">Additional options to customize the deserialization behaviour.</param>
		/// <typeparam name="TTarget">Target type.</typeparam>
		public static TTarget Deserialize<TTarget>(string jsonStr, JsonOptions options = null)
		{
			return (TTarget)Deserialize(typeof(TTarget), jsonStr, options);
		}

		/// <summary>
		/// Process a JSON string value and attempt to deserialize it to an instance of the target type.
		/// </summary>
		/// <param name="targetType">Target type.</param>
		/// <param name="jsonStr">JSON representation of an object.</param>
		/// <param name="options">Additional options to customize the deserialization behaviour.</param>
		public static object Deserialize(Type targetType, string jsonStr, JsonOptions options = null)
		{
			ISerializationDefinition sd = (options?.SerializationDefinition != null) ? options.SerializationDefinition : defaultSerializationDefinition;
			return Serializer.Deserialize(targetType, Deserialize(jsonStr, options), sd);
		}

		/// <summary>
		/// Process a JSON string value and attempt to deserialize it to the given target object.
		/// </summary>
		/// <param name="target">Object to which the JSON data is applied to.</param>
		/// <param name="jsonStr">JSON representation of an object.</param>
		/// <param name="options">Additional options to customize the deserialization behaviour.</param>
		public static void Deserialize(object target, string jsonStr, JsonOptions options = null)
		{
			ISerializationDefinition sd = (options?.SerializationDefinition != null) ? options.SerializationDefinition : defaultSerializationDefinition;
			object result = Deserialize(jsonStr, options);
			Serializer.Deserialize(target, result, sd);
		}

		#region To Json

		private static void ToJson(object obj, SerializationSettings options)
		{
			if (obj == null)
			{
				options.resultStore.Append(NullStr);
			}
			else if (obj is char)
			{
				options.resultStore.Append('"');
				CleanStringToJson(new string((char)obj, 1), options);
				options.resultStore.Append('"');
			}
			else if (obj is string)
			{
				options.resultStore.Append('"');
				CleanStringToJson((string)obj, options);
				options.resultStore.Append('"');
			}
			else if (obj is bool)
			{
				options.resultStore.Append((bool)obj ? TrueStr : FalseStr);
			}
			else if (obj.GetType().IsPrimitive)
			{
				options.resultStore.Append(((IConvertible)obj).ToString(options.serializationDefinition.FormatProvider));
			}
			else if (obj is IDictionary dictionary)
			{
				FromLookupToJson(dictionary, options);
			}
			else if (obj is IList list)
			{
				FromSequenceToJson(list, options);
			}
			else
			{
				try
				{
					// If we can't directly process the object, we attempt to deconstruct it to blocks we can handle.
					object data = Serializer.Serialize(obj, options.serializationDefinition);
					ToJson(data, options);
				}
				catch (System.Exception e)
				{
					Log.Exception(e);
					throw new JsonException("Unexpected JSON building scenario. Failed to serialize object of type {0}.\n{1} message:\n{2}", obj.GetType().Name, e.GetType().Name, e.Message);
				}
			}
		}

		private static void FromLookupToJson(IDictionary obj, SerializationSettings options)
		{
			options.resultStore.Append('{');
			if (!options.compactOutput)
			{
				AdvanceLine(options);
			}

			if (obj.Count > 0)
			{
				IDictionaryEnumerator iterator = obj.GetEnumerator();
				int count = 0;

				while (iterator.MoveNext())
				{
					if (count > 0)
					{
						options.resultStore.Append(',');
						if (!options.compactOutput)
						{
							NewLine(options);
						}
					}
					++count;

					// Key
					options.resultStore.Append('"');
					CleanStringToJson(iterator.Key.ToString(), options);
					options.resultStore.Append("\":");

					if (!options.compactOutput)
					{
						options.resultStore.Append(' ');
					}

					// Value
					ToJson(iterator.Value, options);
				}
			}

			if (!options.compactOutput)
			{
				ReturnLine(options);
			}
			options.resultStore.Append('}');
		}

		private static void FromSequenceToJson(IList obj, SerializationSettings options)
		{
			options.resultStore.Append('[');
			if (!options.compactOutput)
			{
				AdvanceLine(options);
			}

			if (obj.Count > 0)
			{
				IEnumerator iterator = obj.GetEnumerator();
				int count = 0;

				while (iterator.MoveNext())
				{
					if (count > 0)
					{
						options.resultStore.Append(',');
						if (!options.compactOutput)
						{
							NewLine(options);
						}
					}
					++count;

					ToJson(iterator.Current, options);
				}
			}

			if (!options.compactOutput)
			{
				ReturnLine(options);
			}
			options.resultStore.Append(']');
		}

		private static void CleanStringToJson(string str, SerializationSettings options)
		{
			// Regex.Escape is too agressive in removing characters.
			// builder.Append(Regex.Escape(str));

			for (int i = 0; i < str.Length; ++i)
			{
				CleanCharToJson(str[i], options);
			}
		}

		// Based on: https://stackoverflow.com/a/17691629
		private static void CleanCharToJson(char c, SerializationSettings options)
		{
			switch (c)
			{
				case '\\':
				case '"':
					options.resultStore.Append('\\');
					options.resultStore.Append(c);
					break;
				case '/':
					if (options.escapeSlashChar)
					{
						options.resultStore.Append('\\');
					}
					options.resultStore.Append(c);
					break;
				case '\b':
					options.resultStore.Append("\\b");
					break;
				case '\t':
					options.resultStore.Append("\\t");
					break;
				case '\n':
					options.resultStore.Append("\\n");
					break;
				case '\f':
					options.resultStore.Append("\\f");
					break;
				case '\r':
					options.resultStore.Append("\\r");
					break;
				default:
					if (('\x00' <= c) && (c <= '\x1f'))
					{
						options.resultStore.Append(string.Format("\\u{0:D4}", ((int)c)));
					}
					else
					{
						options.resultStore.Append(c);
					}
					break;
			}
		}

		private static void NewLine(SerializationSettings options)
		{
			options.resultStore.Append('\n').Append(options.IndentString);
		}

		private static void AdvanceLine(SerializationSettings options)
		{
			options.IndentationLevel += 1;
			NewLine(options);
		}

		private static void ReturnLine(SerializationSettings options)
		{
			options.IndentationLevel -= 1;
			NewLine(options);
		}

		#endregion // To Json

		#region From Json

		private static object FromJson(ISerializationDefinition serializationDefinition, string str, ref int index)
		{
			if (string.IsNullOrEmpty(str))
			{
				return null;
			}

			SkipWhiteSpace(str, ref index);

			if (index >= str.Length)
			{
				return null;
			}

			char firstChar = str[index];
			switch (firstChar)
			{
				case 'n':
					if (((index + NullStr.Length - 1) >= str.Length) || (str.IndexOf(NullStr, index, NullStr.Length) == -1))
					{
						throw new JsonException("Unexpected end of string. Expected a 'null'-token.");
					}
					index += NullStr.Length;
					return null;
				case 't':
					if (((index + TrueStr.Length - 1) >= str.Length) || (str.IndexOf(TrueStr, index, TrueStr.Length) == -1))
					{
						throw new JsonException("Unexpected end of string. Expected a 'true'-token.");
					}
					index += TrueStr.Length;
					return true;
				case 'f':
					if (((index + FalseStr.Length - 1) >= str.Length) || (str.IndexOf(FalseStr, index, FalseStr.Length) == -1))
					{
						throw new JsonException("Unexpected end of string. Expected a 'false'-token.");
					}
					index += FalseStr.Length;
					return false;
				case '"':
					return FromJsonToString(serializationDefinition, str, ref index);
				case '{':
					{
						if (serializationDefinition is ILookupSerializationDefinition lookupDefinition)
						{
							return FromJsonToLookup(lookupDefinition, str, ref index);
						}
						else
						{
							throw new JsonException("The serialization definition does not implement the {0} interface, but the value contains JSON-object data.", typeof(ILookupSerializationDefinition).Name);
						}
					}
				case '[':
					{
						if (serializationDefinition is IIndexSerializationDefinition indexDefinition)
						{
							return FromJsonToSequence(indexDefinition, str, ref index);
						}
						else
						{
							throw new JsonException("The serialization definition does not implement the {0} interface, but the value contains JSON-array data.", typeof(IIndexSerializationDefinition).Name);
						}
					}
				default:
					if (char.IsDigit(firstChar) || firstChar.Equals('-'))
					{
						return FromJsonToNumerical(serializationDefinition, str, ref index);
					}
					else
					{
						throw new JsonException("Unexpected symbol '{0}'.", firstChar);
					}
			}
		}

		private static string FromJsonToString(ISerializationDefinition serializationDefinition, string str, ref int index)
		{
			// Find the first unescaped '"' character
			int nextIndex = index + 1;
			if (nextIndex >= str.Length)
			{
				throw new JsonException("Unexpected end of string. Incomplete string token.");
			}

			do
			{
				nextIndex = str.IndexOf('"', nextIndex);
				if (nextIndex == -1)
				{
					throw new JsonException("Unexpected end of string. Incomplete string token.");
				}
				else if (!IsCharacterEscaped(str, nextIndex))
				{
					break;
				}
				else
				{
					++nextIndex;
				}

			} while (nextIndex > 0);

			// Extract the value and advance past the closing '"' character.
			string value = str.Substring(index + 1, nextIndex - index - 1);
			index = nextIndex + 1;

			// Unescape values in the string
			return Regex.Unescape(value);
		}

		private static object FromJsonToNumerical(ISerializationDefinition serializationDefinition, string str, ref int index)
		{
			AdvanceUntil(str, ref index, numericalSymbols);

			// Look for a sequence of characters that matches the set
			// of allowed numerical symbols. This is just a fuzzy search at the moment.
			int i = index;
			bool isFloatingPoint = false;
			while (i < str.Length)
			{
				if (!IsCharacterValid(numericalSymbols, str[i]))
				{
					break;
				}

				if (str[i].Equals('.') || str[i].Equals('E') || str[i].Equals('e'))
				{
					isFloatingPoint = true;
				}

				++i;
			}

			// Extract the numerical string and match it with the regex
			string numberStr = str.Substring(index, i - index);
			index = i;
			if (!numericalRegex.IsMatch(numberStr))
			{
				throw new JsonException("The string value '{0}' does not match the JSON-defined numerical pattern.", numberStr);
			}

			// Depending on whether a '.' character is present in the numerical string value,
			// we decide to process the value as a float or int value.
			if (isFloatingPoint)
			{
				double value = double.Parse(numberStr);
				if ((value < float.MaxValue) && (value > float.MinValue))
				{
					return (float)value;
				}
				else
				{
					return value;
				}
			}
			else
			{
				long value = long.Parse(numberStr);
				if ((value < int.MaxValue) && (value > int.MinValue))
				{
					return (int)value;
				}
				else
				{
					return value;
				}
			}
		}

		private static IDictionary FromJsonToLookup(ILookupSerializationDefinition serializationDefinition, string str, ref int index)
		{
			AdvanceUntil(str, ref index, '{');
			IDictionary lookup = serializationDefinition.CreateLookupInstance(0);

			while (index < str.Length)
			{
				AdvanceUntil(str, ref index, new char[] { '}', '"' });
				if (str[index].Equals('}'))
				{
					break;
				}
				else if (!str[index].Equals('"'))
				{
					throw new JsonException("Unexpected '{0}' character. Expected a 'string'-token.", str[index]);
				}

				string key = FromJsonToString(serializationDefinition, str, ref index);

				AdvanceUntil(str, ref index, ':');
				index++;    // Step over the ':' character.

				object value = FromJson(serializationDefinition, str, ref index);

				if (lookup.Contains(key))
				{
					throw new JsonException("The key {0} has already been processed once before.", key);
				}

				lookup.Add(key, value);

				AdvanceUntil(str, ref index, new char[] { ',', '}' });

				// If we come across the character that denotes that more
				// key-value pairs are coming, then we skip the character
				if (str[index].Equals(','))
				{
					index++;
					AdvanceUntil(str, ref index, '"');
				}
				else if (str[index].Equals('}'))
				{
					break;
				}
			}

			index++;    // Step over the '}' character
			return lookup;
		}

		private static IList FromJsonToSequence(IIndexSerializationDefinition serializationDefinition, string str, ref int index)
		{
			AdvanceUntil(str, ref index, '[');
			IList sequence = serializationDefinition.CreateSequenceInstance(0);

			++index;    // Skip over the '[' character

			while (index < str.Length)
			{
				SkipWhiteSpace(str, ref index);
				if (str[index].Equals(']'))
				{
					break;
				}
				else if (str[index].Equals(','))
				{
					throw new JsonException("Unexpected ',' character.");
				}

				object value = FromJson(serializationDefinition, str, ref index);
				sequence.Add(value);

				AdvanceUntil(str, ref index, new char[] { ',', ']' });

				// If we come across the character that denotes that more
				// values are coming, then we skip the character
				if (str[index].Equals(','))
				{
					index++;
				}
				else if (str[index].Equals(']'))
				{
					break;
				}
			}

			index++;    // Step over the ']' character
			return sequence;
		}

		private static void SkipWhiteSpace(string str, ref int index)
		{
			while ((index < str.Length) && char.IsWhiteSpace(str[index]))
			{
				++index;
			}
		}

		private static void AdvanceUntil(string str, ref int index, char c)
		{
			while (index < str.Length)
			{
				if (str[index] == c)
				{
					return;
				}

				++index;
			}

			throw new JsonException("Failed to find required '{0}' character.", c);
		}

		private static void AdvanceUntil(string str, ref int index, char[] cs)
		{
			while (index < str.Length)
			{
				char currentChar = str[index];
				for (int i = 0; i < cs.Length; ++i)
				{
					if (cs[i] == currentChar)
					{
						return;
					}
				}

				++index;
			}

			string errMsg = "Failed to find any of the following required characters: ";
			for (int i = 0; i < cs.Length; ++i)
			{
				errMsg += "'" + cs[i] + "'";
				if (i < (cs.Length - 1))
				{
					errMsg += ", ";
				}
				else
				{
					errMsg += ".";
				}
			}
			throw new JsonException(errMsg);
		}

		private static bool IsCharacterEscaped(string str, int index)
		{
			// Check how many '\'-characters consecutively precede the current character.
			// An uneven amount defines that the character is escaped.
			int i = index;
			do
			{
				--i;
			} while ((i >= 0) && str[i].Equals('\\'));
			return ((index - i + 1) % 2) == 1;
		}

		private static bool IsCharacterValid(char[] characters, char c)
		{
			for (int i = 0; i < characters.Length; ++i)
			{
				if (characters[i] == c)
				{
					return true;
				}
			}

			return false;
		}

		#endregion // From Json

		private class SerializationSettings
		{
			public bool escapeSlashChar = true;
			public bool compactOutput = true;
			public ISerializationDefinition serializationDefinition = null;
			public StringBuilder resultStore = null;

			private int indentLvl = 0;
			private string indentStr = string.Empty;

			public int IndentationLevel
			{
				get { return indentLvl; }
				set
				{
					indentLvl = Math.Max(0, value);
					indentStr = (indentLvl > 0) ? new String('\t', value) : string.Empty;
				}
			}

			public string IndentString
			{
				get { return indentStr; }
			}

			public SerializationSettings(StringBuilder resultStore = null)
			{
				this.resultStore = (resultStore != null) ? resultStore : new StringBuilder(DefaultResultsCacheCapacity);
				this.serializationDefinition = defaultSerializationDefinition;
			}

			public void ApplyOptions(JsonOptions options)
			{
				options.ThrowIfNull(nameof(options));
				escapeSlashChar = options.EscapeSlashCharacter;
				compactOutput = options.CompactOutput;

				if (options.SerializationDefinition != null)
				{
					serializationDefinition = options.SerializationDefinition;
				}
			}
		}
	}
}
