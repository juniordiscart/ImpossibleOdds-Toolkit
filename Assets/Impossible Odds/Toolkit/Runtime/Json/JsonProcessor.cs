using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Caching;

namespace ImpossibleOdds.Json
{
	public static class JsonProcessor
	{
		private const int DefaultResultsCacheCapacity = 1024;
		private const string NullStr = "null";
		private const string TrueStr = "true";
		private const string FalseStr = "false";

		private static readonly JsonSerializationDefinition DefaultSerializationDefinition = new JsonSerializationDefinition();
		private static readonly ILookupSerializationConfiguration DefaultLookupConfiguration = new JsonLookupConfiguration();
		private static readonly ISequenceSerializationConfiguration DefaultSequenceConfiguration = new JsonSequenceConfiguration();
		private static readonly Regex NumericalRegex = new Regex(@"^-?(?:0|[1-9]\d*)(?:\.\d+)?(?:[eE][+-]?\d+)?$");
		private static readonly char[] NumericalSymbols = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '+', 'e', 'E', '.' };
		private static readonly char[] FloatingPointDefiningSymbols = { 'e', 'E', '.' };
		private static readonly char[] SequenceFollowupSymbols = { ',', ']' };
		private static readonly char[] LookupKeyOrCloseSymbols = { '"', '}' };
		private static readonly char[] LookupNextOrCloseSymbols = { ',', '}' };

		private static SerializationState DefaultSerializationState
		{
			get
			{
				SerializationState s = new SerializationState
				{
					escapeSlashChar = true,
					compactOutput = true,
					IndentationLevel = 0,
					serializationDefinition = DefaultSerializationDefinition,

				};
				return s;
			}
		}

		/// <summary>
		/// Enable the parallel processing of the default serialization definition being used.
		/// </summary>
		public static bool ParallelProcessingEnabled
		{
			get => DefaultSerializationDefinition.ParallelProcessingEnabled;
			set => DefaultSerializationDefinition.ParallelProcessingEnabled = value;
		}

		/// <summary>
		/// Serialize the object to a JSON-string.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <param name="options">Options to modify the string output behaviour.</param>
		/// <returns>A JSON representation of the given object.</returns>
		public static string Serialize(object obj, JsonOptions options = null)
		{
			StringBuilder resultStore = new StringBuilder(DefaultResultsCacheCapacity);
			Serialize(obj, resultStore, options);
			return resultStore.ToString();
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
			using StringWriter writer = new StringWriter(resultStore);
			Serialize(obj, writer, options);
		}

		/// <summary>
		/// Serialize the object to a JSON string and write the result directly to the writer.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <param name="writer">Write stream for the JSON result.</param>
		/// <param name="options">Options to modify the string output behaviour.</param>
		public static void Serialize(object obj, TextWriter writer, JsonOptions options = null)
		{
			writer.ThrowIfNull(nameof(writer));

			SerializationState serializationSettings = DefaultSerializationState;
			if (options != null)
			{
				serializationSettings.ApplyOptions(options);
			}

			serializationSettings.writer = writer;
			ToJson(obj, serializationSettings);
		}

		/// <summary>
		/// Process a JSON string to a data structure.
		/// </summary>
		/// <param name="jsonStr">JSON representation of an object.</param>
		/// <param name="options">Additional options to customize the deserialization behaviour.</param>
		public static object Deserialize(string jsonStr, JsonOptions options = null)
		{
			jsonStr.ThrowIfNull(nameof(jsonStr));

			using StringReader reader = new StringReader(jsonStr);
			return Deserialize(reader, options);
		}

		/// <summary>
		/// Process a JSON string to a data structure.
		/// </summary>
		/// <param name="jsonReader">JSON representation of an object.</param>
		/// <param name="options">Additional options to customize the deserialization behaviour.</param>
		public static object Deserialize(TextReader jsonReader, JsonOptions options = null)
		{
			jsonReader.ThrowIfNull(nameof(jsonReader));

			return FromJson(new DeserializationState()
				{
					definition = options?.SerializationDefinition ?? DefaultSerializationDefinition,
					lookupConfiguration = options?.LookupSerializationConfiguration ?? DefaultLookupConfiguration,
					sequenceConfiguration = options?.SequenceSerializationConfiguration ?? DefaultSequenceConfiguration,
					reader = jsonReader,
					buffer = new StringBuilder(),
				});
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
		/// <param name="jsonReader">JSON representation of an object.</param>
		/// <param name="options">Additional options to customize the deserialization behaviour.</param>
		/// <typeparam name="TTarget">Target type.</typeparam>
		public static TTarget Deserialize<TTarget>(TextReader jsonReader, JsonOptions options = null)
		{
			return (TTarget)Deserialize(typeof(TTarget), jsonReader, options);
		}

		/// <summary>
		/// Process a JSON string value and attempt to deserialize it to an instance of the target type.
		/// </summary>
		/// <param name="targetType">Target type.</param>
		/// <param name="jsonStr">JSON representation of an object.</param>
		/// <param name="options">Additional options to customize the deserialization behaviour.</param>
		public static object Deserialize(Type targetType, string jsonStr, JsonOptions options = null)
		{
			targetType.ThrowIfNull(nameof(targetType));
			jsonStr.ThrowIfNull(nameof(jsonStr));

			ISerializationDefinition sd = options?.SerializationDefinition ?? DefaultSerializationDefinition;
			return Serializer.Deserialize(targetType, Deserialize(jsonStr, options), sd);
		}

		/// <summary>
		/// Process a JSON string value and attempt to deserialize it to an instance of the target type.
		/// </summary>
		/// <param name="targetType">Target type.</param>
		/// <param name="jsonReader">JSON representation of an object.</param>
		/// <param name="options">Additional options to customize the deserialization behaviour.</param>
		public static object Deserialize(Type targetType, TextReader jsonReader, JsonOptions options = null)
		{
			targetType.ThrowIfNull(nameof(targetType));
			jsonReader.ThrowIfNull(nameof(jsonReader));

			ISerializationDefinition sd = options?.SerializationDefinition ?? DefaultSerializationDefinition;
			object result = Deserialize(jsonReader, options);
			return Serializer.Deserialize(targetType, result, sd);
		}

		/// <summary>
		/// Process a JSON string value and attempt to deserialize it to the given target object.
		/// </summary>
		/// <param name="target">Object to which the JSON data is applied to.</param>
		/// <param name="jsonStr">JSON representation of an object.</param>
		/// <param name="options">Additional options to customize the deserialization behaviour.</param>
		public static void Deserialize(object target, string jsonStr, JsonOptions options = null)
		{
			target.ThrowIfNull(nameof(target));
			jsonStr.ThrowIfNull(nameof(jsonStr));

			using StringReader reader = new StringReader(jsonStr);
			Deserialize(target, reader, options);
		}

		/// <summary>
		/// Process a JSON string value and attempt to deserialize it to the given target object.
		/// </summary>
		/// <param name="target">Object to which the JSON data is applied to.</param>
		/// <param name="jsonReader">JSON representation of an object.</param>
		/// <param name="options">Additional options to customize the deserialization behaviour.</param>
		public static void Deserialize(object target, TextReader jsonReader, JsonOptions options = null)
		{
			target.ThrowIfNull(nameof(target));
			jsonReader.ThrowIfNull(nameof(jsonReader));

			ISerializationDefinition definition = options?.SerializationDefinition ?? DefaultSerializationDefinition;

			object result = FromJson(
				new DeserializationState()
				{
					definition = definition,
					lookupConfiguration =  options?.LookupSerializationConfiguration ?? DefaultLookupConfiguration,
					sequenceConfiguration = options?.SequenceSerializationConfiguration ?? DefaultSequenceConfiguration,
					buffer = new StringBuilder(),
					reader = jsonReader,
				},
				DataFilterMode.Filter,
				SerializationUtilities.GetTypeMap(target.GetType()));

			Serializer.Deserialize(target, result, definition);
		}

		#region Async
		/// <summary>
		/// Serialize the object async to a JSON-string.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <param name="options">Options to modify the string output behaviour.</param>
		/// <returns>A JSON representation of the given object.</returns>
		public static async Task<string> SerializeAsync(object obj, JsonOptions options = null)
		{
			Task<string> asyncResult = Task.Run(() => Serialize(obj, options));
			await asyncResult;
			return asyncResult.Result;
		}

		/// <summary>
		/// Serialize the object async to a JSON string and store the result directly in the result store.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <param name="resultStore">Write cache for the JSON result.</param>
		/// <param name="options">Options to modify the string output behaviour.</param>
		public static async Task SerializeAsync(object obj, StringBuilder resultStore, JsonOptions options = null)
		{
			await Task.Run(() => Serialize(obj, resultStore, options));
		}

		/// <summary>
		/// Serialize the object async to a JSON string and write the result directly to the writer.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <param name="writer">Write stream for the JSON result.</param>
		/// <param name="options">Options to modify the string output behaviour.</param>
		public static async Task SerializeAsync(object obj, TextWriter writer, JsonOptions options = null)
		{
			await Task.Run(() => Serialize(obj, writer, options));
		}

		/// <summary>
		/// Process a JSON string async to a data structure.
		/// </summary>
		/// <param name="jsonStr">JSON representation of an object.</param>
		/// <param name="options">Additional options to customize the deserialization behaviour.</param>
		public static async Task<object> DeserializeAsync(string jsonStr, JsonOptions options = null)
		{
			Task<object> asyncResult = Task.Run(() => Deserialize(jsonStr, options));
			await asyncResult;
			return asyncResult.Result;
		}

		/// <summary>
		/// Process a JSON string async to a data structure.
		/// </summary>
		/// <param name="jsonReader">JSON representation of an object.</param>
		/// <param name="options">Additional options to customize the deserialization behaviour.</param>
		public static async Task<object> DeserializeAsync(TextReader jsonReader, JsonOptions options = null)
		{
			Task<object> asyncResult = Task.Run(() => Deserialize(jsonReader, options));
			await asyncResult;
			return asyncResult.Result;
		}

		/// <summary>
		/// Process a JSON string value async and attempt to deserialize it to an instance of the target type.
		/// </summary>
		/// <param name="jsonStr">JSON representation of an object.</param>
		/// <param name="options">Additional options to customize the deserialization behaviour.</param>
		/// <typeparam name="TTarget">Target type.</typeparam>
		public static async Task<TTarget> DeserializeAsync<TTarget>(string jsonStr, JsonOptions options = null)
		{
			Task<TTarget> asyncResult = Task.Run(() => Deserialize<TTarget>(jsonStr, options));
			await asyncResult;
			return asyncResult.Result;
		}

		/// <summary>
		/// Process a JSON string value async and attempt to deserialize it to an instance of the target type.
		/// </summary>
		/// <param name="jsonReader">JSON representation of an object.</param>
		/// <param name="options">Additional options to customize the deserialization behaviour.</param>
		/// <typeparam name="TTarget">Target type.</typeparam>
		public static async Task<TTarget> DeserializeAsync<TTarget>(TextReader jsonReader, JsonOptions options = null)
		{
			Task<TTarget> asyncResult = Task.Run(() => Deserialize<TTarget>(jsonReader, options));
			await asyncResult;
			return asyncResult.Result;
		}

		/// <summary>
		/// Process a JSON string value async and attempt to deserialize it to an instance of the target type.
		/// </summary>
		/// <param name="targetType">Target type.</param>
		/// <param name="jsonStr">JSON representation of an object.</param>
		/// <param name="options">Additional options to customize the deserialization behaviour.</param>
		public static async Task<object> DeserializeAsync(Type targetType, string jsonStr, JsonOptions options = null)
		{
			Task<object> asyncResult = Task.Run(() => Deserialize(targetType, jsonStr, options));
			await asyncResult;
			return asyncResult.Result;
		}

		/// <summary>
		/// Process a JSON string value async and attempt to deserialize it to an instance of the target type.
		/// </summary>
		/// <param name="targetType">Target type.</param>
		/// <param name="jsonReader">JSON representation of an object.</param>
		/// <param name="options">Additional options to customize the deserialization behaviour.</param>
		public static async Task<object> DeserializeAsync(Type targetType, TextReader jsonReader, JsonOptions options = null)
		{
			Task<object> asyncResult = Task.Run(() => Deserialize(targetType, jsonReader, options));
			await asyncResult;
			return asyncResult.Result;
		}

		/// <summary>
		/// Process a JSON string value async and attempt to deserialize it to the given target object.
		/// </summary>
		/// <param name="target">Object to which the JSON data is applied to.</param>
		/// <param name="jsonStr">JSON representation of an object.</param>
		/// <param name="options">Additional options to customize the deserialization behaviour.</param>
		public static async Task DeserializeAsync(object target, string jsonStr, JsonOptions options = null)
		{
			await Task.Run(() => Deserialize(target, jsonStr, options));
		}

		/// <summary>
		/// Process a JSON string value async and attempt to deserialize it to the given target object.
		/// </summary>
		/// <param name="target">Object to which the JSON data is applied to.</param>
		/// <param name="jsonReader">JSON representation of an object.</param>
		/// <param name="options">Additional options to customize the deserialization behaviour.</param>
		public static async Task DeserializeAsync(object target, TextReader jsonReader, JsonOptions options = null)
		{
			await Task.Run(() => Deserialize(target, jsonReader, options));
		}
		#endregion

		#region To Json

		private static void ToJson(object obj, SerializationState writer)
		{
			switch (obj)
			{
				case null:
					writer.writer.Write(NullStr);
					break;
				case char c:
					writer.Write('"');
					CleanStringToJson(new string(c, 1), writer);
					writer.Write('"');
					break;
				case string s:
					writer.Write('"');
					CleanStringToJson(s, writer);
					writer.Write('"');
					break;
				case bool b:
					writer.Write(b ? TrueStr : FalseStr);
					break;
				default:
				{
					if (obj.GetType().IsPrimitive)
					{
						writer.Write(((IConvertible)obj).ToString(writer.serializationDefinition.FormatProvider));
					}
					else switch (obj)
					{
						case IDictionary dictionary:
							FromLookupToJson(dictionary, writer);
							break;
						case IList list:
							FromSequenceToJson(list, writer);
							break;
						default:
							try
							{
								// If we can't directly process the object, we attempt to deconstruct it to blocks we can handle.
								object data = Serializer.Serialize(obj, writer.serializationDefinition);
								ToJson(data, writer);
							}
							catch (System.Exception e)
							{
								Log.Exception(e);
								throw new JsonException("Unexpected JSON building scenario. Failed to serialize object of type {0}.\n{1} message:\n{2}", obj.GetType().Name, e.GetType().Name, e.Message);
							}

							break;
					}

					break;
				}
			}
		}

		private static void FromLookupToJson(IDictionary obj, SerializationState writer)
		{
			writer.Write('{');
			writer.AdvanceLine();

			if (obj.Count > 0)
			{
				IDictionaryEnumerator iterator = obj.GetEnumerator();
				int count = 0;

				while (iterator.MoveNext())
				{
					if (count > 0)
					{
						writer.Write(',');
						writer.NewLine();
					}
					++count;

					// Key
					writer.Write('"');
					CleanStringToJson(iterator.Key.ToString(), writer);
					writer.Write("\":");

					if (!writer.compactOutput)
					{
						writer.Write(' ');
					}

					// Value
					ToJson(iterator.Value, writer);
				}
			}

			writer.ReturnLine();
			writer.Write('}');
		}

		private static void FromSequenceToJson(IList obj, SerializationState writer)
		{
			writer.Write('[');
			writer.AdvanceLine();

			if (obj.Count > 0)
			{
				IEnumerator iterator = obj.GetEnumerator();
				int count = 0;

				while (iterator.MoveNext())
				{
					if (count > 0)
					{
						writer.Write(',');
						writer.NewLine();
					}

					++count;
					ToJson(iterator.Current, writer);
				}
			}

			writer.ReturnLine();
			writer.Write(']');
		}

		private static void CleanStringToJson(string str, SerializationState writer)
		{
			// Regex.Escape is too aggressive in removing characters.
			// builder.Append(Regex.Escape(str));
			foreach (char t in str)
			{
				CleanCharToJson(t, writer);
			}
		}

		// Based on: https://stackoverflow.com/a/17691629
		private static void CleanCharToJson(char c, SerializationState writer)
		{
			switch (c)
			{
				case '\\':
				case '"':
					writer.Write('\\');
					writer.Write(c);
					break;
				case '/':
					if (writer.escapeSlashChar)
					{
						writer.Write('\\');
					}
					writer.Write(c);
					break;
				case '\b':
					writer.Write("\\b");
					break;
				case '\t':
					writer.Write("\\t");
					break;
				case '\n':
					writer.Write("\\n");
					break;
				case '\f':
					writer.Write("\\f");
					break;
				case '\r':
					writer.Write("\\r");
					break;
				default:
					if (('\x00' <= c) && (c <= '\x1f'))
					{
						writer.Write($"\\u{((int)c):D4}");
					}
					else
					{
						writer.writer.Write(c);
					}
					break;
			}
		}

		#endregion // To Json

		#region From Json
		private static object FromJson(DeserializationState dv, DataFilterMode filterMode = DataFilterMode.ProcessAll, ISerializationReflectionMap reflectionMap = null)
		{
			if (dv.IsDone)
			{
				return null;
			}

			ReadWhitespace(dv);

			// If, after only reading whitespace, and the reader is already done, then nothing valid was found.
			if (dv.IsDone)
			{
				return null;
			}

			switch (dv.Peek)
			{
				case 'n':   // Null
					ReadValueOrDie(dv, NullStr);
					return null;
				case 't':   // True
					ReadValueOrDie(dv, TrueStr);
					return true;
				case 'f':   // False
					ReadValueOrDie(dv, FalseStr);
					return false;
				case '"':   // String
					switch (filterMode)
					{
						case DataFilterMode.Skip:
							SkipString(dv);
							return null;
						default:
							return ReadString(dv);
					}
				case '{':   // Object
					switch(filterMode)
					{
						case DataFilterMode.ProcessAll:
							return ProcessLookup(dv);
						case DataFilterMode.Filter:
							return FilterLookup(dv, reflectionMap);
						case DataFilterMode.Skip:
							SkipLookup(dv);
							return null;
						default:
							return ProcessLookup(dv);
					}
				case '[':   // Array
					switch(filterMode)
					{
						case DataFilterMode.ProcessAll:
							return ProcessSequence(dv);
						case DataFilterMode.Filter:
							return FilterSequence(dv, reflectionMap);
						case DataFilterMode.Skip:
							SkipSequence(dv);
							return null;
						default:
							return ProcessSequence(dv);
					}
				default:    // Numerical, or unexpected
					if (char.IsDigit(dv.Peek) || Equals(dv.Peek, '-'))
					{
						switch(filterMode)
						{
							case DataFilterMode.Skip:
								SkipNumber(dv);
								return null;
							default:
								return ReadNumber(dv);
						}
					}
					else
					{
						throw new JsonException("Unexpected symbol '{0}'.", dv.Peek);
					}
			}
		}

		private static string ReadString(DeserializationState reader)
		{
			if (!Equals(reader.Peek, '"'))
			{
				throw new JsonException("Expected the '\"'-token to read a string value.");
			}

			reader.buffer.Clear();
			_ = reader.Read;    // Skip the first '"' character.

			do
			{
				// Read until the next '"' character.
				while (!reader.IsDone && !Equals(reader.Peek, '"'))
				{
					reader.buffer.Append(reader.Read);
				}

				if (reader.IsDone)
				{
					throw new JsonException("Unexpected end of string. Incomplete string token: '{0}'.", reader.buffer.ToString());
				}

				bool isEscaped = IsCharacterEscaped(reader);
				if (isEscaped && Equals(reader.Peek, '"'))
				{
					reader.buffer.Append(reader.Read);
				}
				else if (!isEscaped)
				{
					break;
				}
			}
			while (!reader.IsDone);

			// Read the value from the buffer.
			string value = reader.buffer.ToString();
			reader.buffer.Clear();
			_ = reader.Read;    // Read the final '"'-character.

			// Unescape the result.
			return Regex.Unescape(value);
		}

		private static void SkipString(DeserializationState reader)
		{
			if (!Equals(reader.Peek, '"'))
			{
				throw new JsonException("Expected the '\"'-token to read a string value.");
			}

			reader.buffer.Clear();
			_ = reader.Read;    // Skip the first '"' character.

			do
			{
				// Read until the next '"' character.
				while (!reader.IsDone && !Equals(reader.Peek, '"'))
				{
					_ = reader.Read;
				}

				if (reader.IsDone)
				{
					throw new JsonException("Unexpected end of string. Incomplete string token: '{0}'.", reader.buffer.ToString());
				}

				bool isEscaped = IsCharacterEscaped(reader);
				if (isEscaped && Equals(reader.Peek, '"'))
				{
					_ = reader.Read;
				}
				else if (!isEscaped)
				{
					break;
				}
			}
			while (!reader.IsDone);
		}

		private static object ReadNumber(DeserializationState reader)
		{
			ReadUntil(reader, NumericalSymbols);
			reader.buffer.Clear();

			bool isFloatingPoint = false;
			while (!reader.IsDone && IsCharacterValid(NumericalSymbols, reader.Peek))
			{
				if (IsCharacterValid(FloatingPointDefiningSymbols, reader.Peek))
				{
					isFloatingPoint = true;
				}

				reader.buffer.Append(reader.Read);
			}

			string numberStr = reader.buffer.ToString();
			reader.buffer.Clear();

			if (!NumericalRegex.IsMatch(numberStr))
			{
				throw new JsonException("The string value '{0}' does not match the JSON-defined numerical pattern.", numberStr);
			}

			if (isFloatingPoint)
			{
				double value = double.Parse(numberStr, reader.definition.FormatProvider);
				return ((value < float.MaxValue) && (value > float.MinValue)) ? (float)value : value;
			}
			else
			{
				long value = long.Parse(numberStr, reader.definition.FormatProvider);
				return ((value < int.MaxValue) && (value > int.MinValue)) ? (int)value : value;
			}
		}

		private static void SkipNumber(DeserializationState reader)
		{
			ReadUntil(reader, NumericalSymbols);
			reader.buffer.Clear();

			while (!reader.IsDone && IsCharacterValid(NumericalSymbols, reader.Peek))
			{
				_ = reader.Read;
			}
		}

		private static IDictionary ProcessLookup(DeserializationState reader)
		{
			if (reader.lookupConfiguration is null)
			{
				throw new JsonException("The JSON data contains a JSON-object, but no lookup configuration has been provided.");
			}

			IDictionary lookup = reader.lookupConfiguration.CreateLookupInstance(0);

			_ = ReadUntil(reader, '{').Read; // Skip over the '{' character.

			while (!reader.IsDone)
			{
				ReadUntil(reader, LookupKeyOrCloseSymbols);
				if (Equals(reader.Peek, '}'))
				{
					break;
				}

				if (!Equals(reader.Peek, '"'))
				{
					throw new JsonException("Unexpected '{0}' character. Expected a string-token.", reader.Peek);
				}

				string key = ReadString(reader);
				_ = ReadUntil(reader, ':').Read; // Step over the ':' character.
				object value = FromJson(reader);

				if (lookup != null)
				{
					if (lookup.Contains(key))
					{
						throw new JsonException("The key '{0}' is present multiple times in the JSON object.", key);
					}

					lookup.Add(key, value);
				}

				ReadUntil(reader, LookupNextOrCloseSymbols);

				if (Equals(reader.Peek, ','))
				{
					_ = reader.Read;    // Step over the ',' character.
					ReadUntil(reader, '"');  // We expect an object key next, so advance to the '"' character.
				}
				else if (Equals(reader.Peek, '}'))
				{
					break;
				}
			}

			_ = reader.Read;    // Step over the closing '}' character.
			return lookup;
		}

		private static IDictionary FilterLookup(DeserializationState reader, ISerializationReflectionMap reflectionMap)
		{
			if (reader.lookupConfiguration is null)
			{
				throw new JsonException("The JSON data contains a JSON-object, but no lookup configuration has been provided.");
			}

			reflectionMap.ThrowIfNull(nameof(reflectionMap));
			ISerializableMember[] members = reflectionMap.GetUniqueSerializableMembers(reader.lookupConfiguration.MemberAttribute);

			IDictionary lookup = reader.lookupConfiguration.CreateLookupInstance(0);
			_ = ReadUntil(reader, '{').Read;    // Skip over the '{' character.

			while (!reader.IsDone)
			{
				ReadUntil(reader, LookupKeyOrCloseSymbols);
				if (Equals(reader.Peek, '}'))
				{
					break;
				}

				if (!Equals(reader.Peek, '"'))
				{
					throw new JsonException("Unexpected '{0}' character. Expected a string-token.", reader.Peek);
				}

				string key = ReadString(reader);
				if (lookup.Contains(key))
				{
					throw new JsonException("The key '{0}' is present multiple times in the JSON object.", key);
				}

				_ = ReadUntil(reader, ':').Read; // Step over the ':' character.

				// If the member key exists, then the value should be processed and added. Otherwise, the value can be skipped.
				if (MemberKeyExists(members, key))
				{
					lookup.Add(key, FromJson(reader, DataFilterMode.ProcessAll));
				}
				else
				{
					FromJson(reader, DataFilterMode.Skip);
				}

				ReadUntil(reader, LookupNextOrCloseSymbols);

				if (Equals(reader.Peek, ','))
				{
					_ = reader.Read;    // Step over the ',' character.
					ReadUntil(reader, '"');  // We expect an object key next, so advance to the '"' character.
				}
				else if (Equals(reader.Peek, '}'))
				{
					break;
				}
			}

			_ = reader.Read;    // Step over the closing '}' character.
			return lookup;

			bool MemberKeyExists(ISerializableMember[] _members, string key)
			{
				foreach(ISerializableMember member in _members)
				{
					if ((!(member.Attribute is ILookupParameter lookupParameter)))
					{
						continue;
					}

					string memberKey = lookupParameter.Key as string;

					if (memberKey.IsNullOrEmpty())
					{
						memberKey = member.Member.Name;
					}

					if (string.Equals(key, memberKey))
					{
						return true;
					}
				}

				return false;
			}
		}

		private static void SkipLookup(DeserializationState reader)
		{
			_ = ReadUntil(reader, '{').Read; // Skip over the '{' character.

			while (!reader.IsDone)
			{
				ReadUntil(reader, LookupKeyOrCloseSymbols);
				if (Equals(reader.Peek, '}'))
				{
					break;
				}

				if (!Equals(reader.Peek, '"'))
				{
					throw new JsonException("Unexpected '{0}' character. Expected a string-token.", reader.Peek);
				}

				SkipString(reader);	// Read and skip the key.
				_ = ReadUntil(reader, ':').Read; // Step over the ':' character.
				FromJson(reader, DataFilterMode.Skip); // Read and skip the value.

				ReadUntil(reader, LookupNextOrCloseSymbols);
				if (Equals(reader.Peek, ','))
				{
					_ = reader.Read;    // Step over the ',' character.
					ReadUntil(reader, '"');  // We expect an object key next, so advance to the '"' character.
				}
				else if (Equals(reader.Peek, '}'))
				{
					break;
				}
			}

			_ = reader.Read;    // Step over the closing '}' character.
		}

		private static IList ProcessSequence(DeserializationState reader)
		{
			if (reader.sequenceConfiguration is null)
			{
				throw new JsonException("The JSON data contains a JSON-array, but no sequence configuration has been provided.");
			}

			IList sequence = reader.sequenceConfiguration.CreateSequenceInstance(0);
			_ = ReadUntil(reader, '[').Read; // Skip over the starting '[' character.

			while (!reader.IsDone)
			{
				ReadWhitespace(reader);
				if (Equals(reader.Peek, ']'))
				{
					break;  // End of sequence reached.
				}

				if (Equals(reader.Peek, ','))
				{
					throw new JsonException("Unexpected ',' character.");
				}

				sequence.Add(FromJson(reader));

				ReadUntil(reader, SequenceFollowupSymbols);
				if (Equals(reader.Peek, ','))
				{
					_ = reader.Read;    // Skip the over ',' character.
				}
				else if (Equals(reader.Peek, ']'))
				{
					break;  // End of sequence reached.
				}
			}

			_ = reader.Read;    // Skip over the closing ']' character.
			return sequence;
		}

		private static IList FilterSequence(DeserializationState reader, ISerializationReflectionMap reflectionMap)
		{
			if (reader.sequenceConfiguration is null)
			{
				throw new JsonException("The JSON data contains a JSON-array, but no sequence configuration has been provided.");
			}

			reflectionMap.ThrowIfNull(nameof(reflectionMap));

			IList sequence = reader.sequenceConfiguration.CreateSequenceInstance(0);
			_ = ReadUntil(reader, '[').Read; // Skip over the starting '[' character.

			ISerializableMember[] members = reflectionMap.GetUniqueSerializableMembers(reader.sequenceConfiguration.MemberAttribute);
			int maxIndex = members.Where(m => m.Attribute is ISequenceParameter).Max(m => ((ISequenceParameter)m.Attribute).Index);

			int currentIndex = 0;
			while (!reader.IsDone)
			{
				ReadWhitespace(reader);
				if (Equals(reader.Peek, ']'))
				{
					break;  // End of sequence reached.
				}

				if (Equals(reader.Peek, ','))
				{
					throw new JsonException("Unexpected ',' character.");
				}

				if (currentIndex <= maxIndex)
				{
					sequence.Add(FromJson(reader));
				}
				else
				{
					FromJson(reader, DataFilterMode.Skip);
				}

				ReadUntil(reader, SequenceFollowupSymbols);

				if (Equals(reader.Peek, ','))
				{
					_ = reader.Read;    // Skip the over ',' character.
				}
				else if (Equals(reader.Peek, ']'))
				{
					break;  // End of sequence reached.
				}

				currentIndex++;
			}

			_ = reader.Read;    // Skip over the closing ']' character.
			return sequence;
		}

		private static void SkipSequence(DeserializationState reader)
		{
			_ = ReadUntil(reader, '[').Read; // Skip over the starting '[' character.

			while (!reader.IsDone)
			{
				ReadWhitespace(reader);
				if (Equals(reader.Peek, ']'))
				{
					break;  // End of sequence reached.
				}

				if (Equals(reader.Peek, ','))
				{
					throw new JsonException("Unexpected ',' character.");
				}

				FromJson(reader, DataFilterMode.Skip);
				ReadUntil(reader, SequenceFollowupSymbols);

				if (Equals(reader.Peek, ','))
				{
					_ = reader.Read;    // Skip the over ',' character.
				}
				else if (Equals(reader.Peek, ']'))
				{
					break;  // End of sequence reached.
				}
			}

			_ = reader.Read;    // Skip over the closing ']' character.
		}

		private static void ReadWhitespace(DeserializationState reader)
		{
			// While there's data to be read and the character is considered a whitespace character, advance the cursor.
			while (!reader.IsDone && char.IsWhiteSpace(reader.Peek))
			{
				_ = reader.Read;
			}
		}

		private static DeserializationState ReadUntil(DeserializationState reader, char c)
		{
			while (!reader.IsDone)
			{
				if (reader.Peek == c)
				{
					return reader;
				}

				_ = reader.Read;
			}

			throw new JsonException("Failed to find required '{0}' character.", c);
		}

		private static DeserializationState ReadUntil(DeserializationState reader, char[] cs)
		{
			while (!reader.IsDone)
			{
				if (IsCharacterValid(cs, reader.Peek))
				{
					return reader;
				}

				_ = reader.Read;
			}

			string errMsg = "Failed to find any of the following required characters: ";
			for (int i = 0; i < cs.Length; ++i)
			{
				errMsg += "'" + cs[i] + "'";
				errMsg += (i < (cs.Length - 1)) ? ", " : ".";
			}

			throw new JsonException(errMsg);
		}

		/// <summary>
		/// Checks whether the last characters in the buffer form an even or uneven set of '\' characters.
		/// An even count of '\' characters denotes the next character is not escaped. Uneven means it is escaped.
		/// </summary>
		private static bool IsCharacterEscaped(DeserializationState reader)
		{
			int escapeCount = 0;
			for (int i = (reader.buffer.Length - 1); i >= 0; --i)
			{
				if (Equals(reader.buffer[i], '\\'))
				{
					escapeCount++;
				}
				else
				{
					break;
				}
			}

			return (escapeCount % 2) == 1;
		}

		/// <summary>
		/// Reads the value from the reader. If it doesn't match the expected value, an exception is thrown.
		/// </summary>
		private static void ReadValueOrDie(DeserializationState reader, string expectedValue)
		{
			foreach (char c in expectedValue)
			{
				if (reader.Peek != c)
				{
					throw new JsonException("Unexpected string value. Expected to read '{0}'.", expectedValue);
				}

				_ = reader.Read;
			}
		}

		private static bool IsCharacterValid(char[] characters, char character)
		{
			return Array.Exists(characters, c => c == character);
		}
		#endregion // From Json
	}
}