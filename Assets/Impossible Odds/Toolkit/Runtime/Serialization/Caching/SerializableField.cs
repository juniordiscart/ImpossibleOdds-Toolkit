using System;
using System.Reflection;
using ImpossibleOdds.ReflectionCaching;

namespace ImpossibleOdds.Serialization.Caching
{
	public class SerializableField : ISerializableField
	{
		public SerializableField(FieldInfo field, Attribute serializationAttribute)
		{
			field.ThrowIfNull(nameof(field));
			serializationAttribute.ThrowIfNull(nameof(serializationAttribute));

			Field = field;
			Attribute = serializationAttribute;
		}

		/// <inheritdoc />
		public FieldInfo Field { get; }

		/// <inheritdoc />
		public Type MemberType => Field.FieldType;

		/// <inheritdoc />
		public Attribute Attribute { get; }

		/// <inheritdoc />
		public Type AttributeType => Attribute.GetType();

		/// <inheritdoc />
		MemberInfo IMemberAttributePair.Member => Field;

		/// <inheritdoc />
		public object GetValue(object source)
		{
			return Field.GetValue(source);
		}

		/// <inheritdoc />
		public void SetValue(object source, object value)
		{
			Field.SetValue(source, value);
		}
	}
}