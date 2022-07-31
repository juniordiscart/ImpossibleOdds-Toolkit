namespace ImpossibleOdds.Json
{
	using System;
	using System.IO;
	using ImpossibleOdds.Serialization;

	internal struct SerializationState
	{
		public bool escapeSlashChar;
		public bool compactOutput;
		public ISerializationDefinition serializationDefinition;
		public TextWriter writer;

		private int indentLvl;
		private string indentStr;

		/// <summary>
		/// The current indentation level of the the writer.
		/// </summary>
		public int IndentationLevel
		{
			get => indentLvl;
			set
			{
				indentLvl = Math.Max(0, value);
				indentStr = (indentLvl > 0) ? new String('\t', value) : string.Empty;
			}
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

		public SerializationState Write(string value)
		{
			writer.Write(value);
			return this;
		}

		public SerializationState Write(char value)
		{
			writer.Write(value);
			return this;
		}

		public void NewLine()
		{
			if (!compactOutput)
			{
				writer.WriteLine();
				writer.Write(indentStr);
			}
		}

		public void AdvanceLine()
		{
			IndentationLevel += 1;
			NewLine();
		}

		public void ReturnLine()
		{
			IndentationLevel -= 1;
			NewLine();
		}
	}
}
