namespace ImpossibleOdds.Json
{
	using System.IO;
	using System.Text;
	using ImpossibleOdds.Serialization;

	internal struct DeserializationState
	{
		public bool IsDone
		{
			get => reader.Peek() < 0;
		}

		public char Peek
		{
			get => (char)reader.Peek();
		}

		public char Read
		{
			get => (char)reader.Read();
		}

		public ISerializationDefinition definition;
		public TextReader reader;
		public StringBuilder buffer;
	}
}
