namespace ImpossibleOdds.Serialization.Caching
{
	using System;
	using System.Reflection;
	using ImpossibleOdds.ReflectionCaching;

	public class SerializableField : ISerializableField
	{
		private readonly FieldInfo field;
		private readonly Attribute attribute;

		public SerializableField(FieldInfo field, Attribute serializationAttribute)
		{
			field.ThrowIfNull(nameof(field));
			serializationAttribute.ThrowIfNull(nameof(serializationAttribute));

			this.field = field;
			this.attribute = serializationAttribute;
		}

		/// <inheritdoc />
		public FieldInfo Field
		{
			get => field;
		}

		/// <inheritdoc />
		public Type MemberType
		{
			get => field.FieldType;
		}

		/// <inheritdoc />
		public Attribute Attribute
		{
			get => attribute;
		}

		/// <inheritdoc />
		public Type AttributeType
		{
			get => attribute.GetType();
		}

		/// <inheritdoc />
		MemberInfo IMemberAttributePair.Member
		{
			get => field;
		}

		/// <inheritdoc />
		public object GetValue(object source)
		{
			return field.GetValue(source);
		}

		/// <inheritdoc />
		public void SetValue(object source, object value)
		{
			field.SetValue(source, value);
		}
	}
}
