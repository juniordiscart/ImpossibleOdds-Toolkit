namespace ImpossibleOdds.Serialization.Processors
{
	using System;

	/// <summary>
	/// A deserialization processor that handles null values.
	/// If the target is a nullable type, null will be returned, otherwise, if the target
	/// is a value type, the default value for that type will be set on the target.
	/// </summary>
	public class NullValueProcessor : IDeserializationProcessor
	{
		private readonly ISerializationDefinition definition = null;

		public ISerializationDefinition Definition
		{
			get => definition;
		}

		public NullValueProcessor(ISerializationDefinition definition)
		{
			definition.ThrowIfNull(nameof(definition));
			this.definition = definition;
		}

		public bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
		{
			targetType.ThrowIfNull(nameof(targetType));

			if (dataToDeserialize != null)
			{
				deserializedResult = null;
				return false;
			}

			if (targetType.IsValueType)
			{
				deserializedResult = Activator.CreateInstance(targetType);
			}
			else
			{
				deserializedResult = null;
			}

			return true;
		}
	}
}
