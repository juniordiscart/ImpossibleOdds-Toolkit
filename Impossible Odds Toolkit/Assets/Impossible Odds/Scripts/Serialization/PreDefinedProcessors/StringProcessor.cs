﻿namespace ImpossibleOdds.Serialization.Processors
{
	using System;

	/// <summary>
	/// A simple deserialization-only processor to process a value to a string.
	/// </summary>
	public class StringProcessor : AbstractProcessor, IDeserializationProcessor
	{
		public StringProcessor(ISerializationDefinition definition)
		: base(definition)
		{ }

		/// <summary>
		/// Attempts to deserialize a string value to a string or to a system supported type using the System.Convert method.
		/// </summary>
		/// <param name="targetType">The target type to deserialize the given data.</param>
		/// <param name="dataToDeserialize">The data deserialize and apply to the result.</param>
		/// <param name="deserializedResult">The result unto which the data is applied.</param>
		/// <returns>True if deserialization is compatible and accepted, false otherwise.</returns>
		public bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
		{
			if (!typeof(string).IsAssignableFrom(targetType))
			{
				deserializedResult = null;
				return false;
			}

			if (dataToDeserialize is string)
			{
				deserializedResult = dataToDeserialize;
				return true;
			}

			deserializedResult = Convert.ChangeType(dataToDeserialize, targetType);
			return true;
		}
	}
}
