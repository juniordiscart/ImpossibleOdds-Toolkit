namespace ImpossibleOdds.Json
{
	using System.IO;
	using System.Text;
	using ImpossibleOdds.Serialization;

	internal struct DeserializationState
	{
		/// <summary>
		/// Is the reader done reading?
		/// </summary>
		public bool IsDone
		{
			get => reader.Peek() < 0;
		}

		/// <summary>
		/// Peek at the next character to be read from the reader.
		/// </summary>
		public char Peek
		{
			get => (char)reader.Peek();
		}

		/// <summary>
		/// Read a single character and advance the cursor in the reader.
		/// </summary>
		public char Read
		{
			get => (char)reader.Read();
		}

		public ISerializationDefinition definition;
		public TextReader reader;
		public StringBuilder buffer;
	}
}
