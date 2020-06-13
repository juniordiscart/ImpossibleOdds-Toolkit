namespace ImpossibleOdds.Json
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Runtime.Serialization;

	using ImpossibleOdds.Serialization;

	public static class JsonProcessor
	{
		private const int DEFAULT_CAPACITY = 1024;
		private const string NULL_STR = "null";
		private const string TRUE_STR = "true";
		private const string FALSE_STR = "false";

		private static Regex numericalRegex = new Regex(@"^-?(?:0|[1-9]\d*)(?:\.\d+)?(?:[eE][+-]?\d+)?$");
		private static JsonDefaultSerializationDefinition defaultSerializationDefinition = new JsonDefaultSerializationDefinition();

		/// <summary>
		/// Processes an object to a JSON-compliant string.
		/// </summary>
		/// <param name="obj">Object to be processed.</param>
		/// <param name="options">Options to modify the string output behaviour.</param>
		/// <returns></returns>
		public static string Serialize(object obj, JsonOptions options = null)
		{
			StringBuilder builder = new StringBuilder(DEFAULT_CAPACITY);
			RuntimeOptions runtimeOptions = (options != null) ? new RuntimeOptions(options) : new RuntimeOptions();
			ToJson(obj, builder, runtimeOptions);
			return builder.ToString();
		}

		/// <summary>
		/// Processes a JSON-compliant string value to a native data structure.
		/// </summary>
		/// <param name="jsonStr">JSON-compliant string.</param>
		/// <returns></returns>
		public static object Deserialize(string jsonStr)
		{
			return Deserialize<Dictionary<string, object>, List<object>>(jsonStr);
		}

		/// <summary>
		/// Processes a JSON-compliant string value and attempts to deserialize it to an instance of type TTarget.
		/// </summary>
		/// <param name="jsonStr">JSON-compliant string.</param>
		/// <typeparam name="TTarget">Target type.</typeparam>
		/// <returns></returns>
		public static TTarget Deserialize<TTarget>(string jsonStr)
		{
			Type targetType = typeof(TTarget);

			if (targetType.IsAbstract || targetType.IsInterface)
			{
				throw new JsonException(string.Format("Cannot create instance of type {0} because it is either astract or an interface."));
			}

			TTarget target = (TTarget)FormatterServices.GetUninitializedObject(targetType);
			Deserialize(target, jsonStr);
			return target;
		}

		/// <summary>
		/// Processes a JSON-compliant string value and attempts to deserialize it to the given target object.
		/// </summary>
		/// <param name="target">Object to which the JSON data is applied to.</param>
		/// <param name="jsonStr">JSON-compliant string.</param>
		public static void Deserialize(object target, string jsonStr)
		{
			object result = Deserialize(jsonStr);
			Serializer.Deserialize(target, result, defaultSerializationDefinition);
		}

		/// <summary>
		/// Processes a JSON-compliant string value to the provided custom data structures.
		/// </summary>
		/// <param name="jsonStr">JSON-compliant string.</param>
		/// <typeparam name="TJsonObject">Custom JSON object type.</typeparam>
		/// <typeparam name="TJsonArray">Custom JSON array type.</typeparam>
		/// <returns></returns>
		public static object Deserialize<TJsonObject, TJsonArray>(string jsonStr)
		where TJsonObject : IDictionary, new()
		where TJsonArray : IList, new()
		{
			int index = 0;
			return FromJson<TJsonObject, TJsonArray>(jsonStr, ref index);
		}

		/// <summary>
		/// Processes a JSON-compliant string value and attempts to deserialize it to the given target object using the provided data structures and serialization defintion.
		/// </summary>
		/// <param name="target">Object to which the JSON data is applied to.</param>
		/// <param name="jsonStr">JSON-compliant string.</param>
		/// <param name="serializationDefinition">Custom serialization definition.</param>
		/// <typeparam name="TJsonObject">Custom JSON object type.</typeparam>
		/// <typeparam name="TJsonArray">Custom JSON array type.</typeparam>
		public static void Deserialize<TJsonObject, TJsonArray>(object target, string jsonStr, ISerializationDefinition serializationDefinition)
		where TJsonObject : IDictionary, new()
		where TJsonArray : IList, new()
		{
			object result = Deserialize<TJsonObject, TJsonArray>(jsonStr);
			Serializer.Deserialize(target, result, serializationDefinition);
		}

		#region To Json

		private static void ToJson(object obj, StringBuilder builder, RuntimeOptions options)
		{
			if (obj == null)
			{
				builder.Append(NULL_STR);
			}
			else if (obj is char)
			{
				builder.Append('"');
				CleanStringToJson(new string((char)obj, 1), builder, options);
				builder.Append('"');
			}
			else if (obj is string)
			{
				builder.Append('"');
				CleanStringToJson((string)obj, builder, options);
				builder.Append('"');
			}
			else if (obj is bool)
			{
				builder.Append((bool)obj ? TRUE_STR : FALSE_STR);
			}
			else if (obj.GetType().IsPrimitive)
			{
				builder.Append(obj);
			}
			else if (obj is IDictionary)
			{
				FromLookupToJson(obj as IDictionary, builder, options);
			}
			else if (obj is IList)
			{
				FromSequenceToJson(obj as IList, builder, options);
			}
			else
			{
				try
				{
					// If we can't directly process the object, we attempt to deconstruct it to blocks we can handle.
					object data = Serializer.Serialize(obj, options.serializationDefinition);
					ToJson(data, builder, options);
				}
				catch (System.Exception e)
				{
					Debug.Exception(e);
					throw new JsonException(string.Format("Unexpected JSON building scenario. Failed to serialize object of type {0}.\n{1} message:\n{2}", obj.GetType().Name, e.GetType().Name, e.Message));
				}
			}
		}

		private static void FromLookupToJson(IDictionary obj, StringBuilder builder, RuntimeOptions options)
		{
			builder.Append('{');
			if (options.prettyPrint)
			{
				AdvanceLine(builder, options);
			}

			if (obj.Count > 0)
			{
				IDictionaryEnumerator iterator = obj.GetEnumerator();
				int count = 0;

				while (iterator.MoveNext())
				{
					if (count > 0)
					{
						builder.Append(',');
						if (options.prettyPrint)
						{
							NewLine(builder, options);
						}
					}
					++count;

					// Key
					builder.Append('"');
					CleanStringToJson(iterator.Key.ToString(), builder, options);
					builder.Append("\":");

					if (options.prettyPrint)
					{
						builder.Append(' ');
					}

					// Value
					ToJson(iterator.Value, builder, options);
				}
			}

			if (options.prettyPrint)
			{
				ReturnLine(builder, options);
			}
			builder.Append('}');
		}

		private static void FromSequenceToJson(IList obj, StringBuilder builder, RuntimeOptions options)
		{
			builder.Append('[');
			if (options.prettyPrint)
			{
				AdvanceLine(builder, options);
			}

			if (obj.Count > 0)
			{
				IEnumerator iterator = obj.GetEnumerator();
				int count = 0;

				while (iterator.MoveNext())
				{
					if (count > 0)
					{
						builder.Append(',');
						if (options.prettyPrint)
						{
							NewLine(builder, options);
						}
					}
					++count;

					ToJson(iterator.Current, builder, options);
				}
			}

			if (options.prettyPrint)
			{
				ReturnLine(builder, options);
			}
			builder.Append(']');
		}

		private static void CleanStringToJson(string str, StringBuilder builder, RuntimeOptions options)
		{
			// Regex.Escape is too agressive in removing characters.
			// builder.Append(Regex.Escape(str));

			for (int i = 0; i < str.Length; ++i)
			{
				CleanCharToJson(str[i], builder, options);
			}
		}

		// Based on: https://stackoverflow.com/a/17691629
		private static void CleanCharToJson(char c, StringBuilder builder, RuntimeOptions options)
		{
			switch (c)
			{
				case '\\':
				case '"':
					builder.Append('\\');
					builder.Append(c);
					break;
				case '/':
					if (options.escapeSlashChar)
					{
						builder.Append('\\');
					}
					builder.Append(c);
					break;
				case '\b':
					builder.Append("\\b");
					break;
				case '\t':
					builder.Append("\\t");
					break;
				case '\n':
					builder.Append("\\n");
					break;
				case '\f':
					builder.Append("\\f");
					break;
				case '\r':
					builder.Append("\\r");
					break;
				default:
					if (('\x00' <= c) && (c <= '\x1f'))
					{
						builder.Append(string.Format("\\u{0:D4}", ((int)c)));
					}
					else
					{
						builder.Append(c);
					}
					break;
			}
		}

		private static void NewLine(StringBuilder builder, RuntimeOptions options)
		{
			builder.Append('\n').Append(options.IndentString);
		}

		private static void AdvanceLine(StringBuilder builder, RuntimeOptions options)
		{
			options.IndentationLevel += 1;
			NewLine(builder, options);
		}

		private static void ReturnLine(StringBuilder builder, RuntimeOptions options)
		{
			options.IndentationLevel -= 1;
			NewLine(builder, options);
		}

		#endregion // To Json

		#region From Json

		private static object FromJson<TJsonObject, TJsonArray>(string str, ref int index)
		where TJsonObject : IDictionary, new()
		where TJsonArray : IList, new()
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
					if (((index + NULL_STR.Length - 1) >= str.Length) || (str.IndexOf(NULL_STR, index, NULL_STR.Length) == -1))
					{
						throw new JsonException("Unexpected end of string. Expected a 'null'-token.");
					}
					index += NULL_STR.Length;
					return null;
				case 't':
					if (((index + TRUE_STR.Length - 1) >= str.Length) || (str.IndexOf(TRUE_STR, index, TRUE_STR.Length) == -1))
					{
						throw new JsonException("Unexpected end of string. Expected a 'true'-token.");
					}
					index += TRUE_STR.Length;
					return true;
				case 'f':
					if (((index + FALSE_STR.Length - 1) >= str.Length) || (str.IndexOf(FALSE_STR, index, FALSE_STR.Length) == -1))
					{
						throw new JsonException("Unexpected end of string. Expected a 'false'-token.");
					}
					index += FALSE_STR.Length;
					return false;
				case '"':
					return FromJsonToString(str, ref index);
				case '{':
					return FromJsonToLookup<TJsonObject, TJsonArray>(str, ref index);
				case '[':
					return FromJsonToSequence<TJsonObject, TJsonArray>(str, ref index);
				default:
					if (char.IsDigit(firstChar) || firstChar.Equals('-'))
					{
						return FromJsonToNumerical(str, ref index);
					}
					else
					{
						throw new JsonException(string.Format("Unexpected symbol '{0}'.", firstChar));
					}
			}
		}

		private static string FromJsonToString(string str, ref int index)
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

		private static object FromJsonToNumerical(string str, ref int index)
		{
			char[] numericalSymbols = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '+', 'e', 'E', '.' };

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
				throw new JsonException(string.Format("The string value '{0}' does not match the JSON-defined numerical pattern.", numberStr));
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

		private static TJsonObject FromJsonToLookup<TJsonObject, TJsonArray>(string str, ref int index)
		where TJsonObject : IDictionary, new()
		where TJsonArray : IList, new()
		{
			AdvanceUntil(str, ref index, '{');
			TJsonObject lookup = new TJsonObject();

			while (index < str.Length)
			{
				AdvanceUntil(str, ref index, new char[] { '}', '"' });
				if (str[index].Equals('}'))
				{
					break;
				}
				else if (!str[index].Equals('"'))
				{
					throw new JsonException(string.Format("Unexpected '{0}' character. Expected a 'string'-token.", str[index]));
				}

				string key = FromJsonToString(str, ref index);

				AdvanceUntil(str, ref index, ':');
				index++;    // Step over the ':' character.

				object value = FromJson<TJsonObject, TJsonArray>(str, ref index);

				if (lookup.Contains(key))
				{
					throw new JsonException(string.Format("The key {0} has already been processed once before.", key));
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

		private static TJsonArray FromJsonToSequence<TJsonObject, TJsonArray>(string str, ref int index)
		where TJsonObject : IDictionary, new()
		where TJsonArray : IList, new()
		{
			AdvanceUntil(str, ref index, '[');
			TJsonArray sequence = new TJsonArray();

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

				object value = FromJson<TJsonObject, TJsonArray>(str, ref index);
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

			throw new JsonException(string.Format("Failed to find required '{0}' character.", c));
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

		private class RuntimeOptions
		{
			public readonly bool escapeSlashChar;
			public readonly bool prettyPrint;
			public readonly ISerializationDefinition serializationDefinition;

			private int indentLvl;
			private string indentStr;

			public int IndentationLevel
			{
				get { return indentLvl; }
				set
				{
					indentLvl = Math.Max(0, value);
					indentStr = new String('\t', value);
				}
			}

			public string IndentString
			{
				get { return indentStr; }
			}

			public RuntimeOptions()
			{
				escapeSlashChar = true;
				prettyPrint = false;
				serializationDefinition = defaultSerializationDefinition;
			}

			public RuntimeOptions(JsonOptions options)
			{
				escapeSlashChar = options.EscapeSlashCharacter;
				prettyPrint = !options.Minify;
				serializationDefinition = (options.CustomSerializationDefinition != null) ? options.CustomSerializationDefinition : defaultSerializationDefinition;
			}
		}
	}
}
