using System.IO;
using System.Text;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Json
{
	internal struct DeserializationState
	{
		public ISerializationDefinition definition;
		public ILookupSerializationConfiguration lookupConfiguration;
		public ISequenceSerializationConfiguration sequenceConfiguration;
		public TextReader reader;
		public StringBuilder buffer;

		/// <summary>
		/// Is the reader done reading?
		/// </summary>
		public bool IsDone => reader.Peek() < 0;

		/// <summary>
		/// Peek at the next character to be read from the reader.
		/// </summary>
		public char Peek => (char)reader.Peek();

		/// <summary>
		/// Read a single character and advance the cursor in the reader.
		/// </summary>
		public char Read => (char)reader.Read();
	}
}