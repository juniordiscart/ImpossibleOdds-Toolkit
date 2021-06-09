namespace ImpossibleOdds.Serialization.Caching
{
	using System;
	using System.Reflection;

	public struct FieldAttributeTuple : IMemberAttributeTuple<FieldInfo>
	{
		private readonly FieldInfo field;
		private readonly Attribute attribute;
		private readonly IRequiredParameter requiredParameter;

		/// <summary>
		/// The registered field.
		/// </summary>
		public FieldInfo Field
		{
			get { return field; }
		}

		/// <inheritdoc />
		public Type MemberType
		{
			get { return field.FieldType; }
		}

		/// <inheritdoc />
		public FieldInfo Member
		{
			get { return field; }
		}

		/// <inheritdoc />
		public Attribute Attribute
		{
			get { return attribute; }
		}

		/// <inheritdoc />
		MemberInfo IMemberAttributeTuple.Member
		{
			get { return Member; }
		}

		/// <inheritdoc />
		public bool IsRequiredParameter
		{
			get { return requiredParameter != null; }
		}

		/// <inheritdoc />
		public IRequiredParameter RequiredParameter
		{
			get { return requiredParameter; }
		}

		public FieldAttributeTuple(FieldInfo field, Attribute serializationAttribute, Type requiredAttributeType = null)
		{
			field.ThrowIfNull(nameof(field));
			serializationAttribute.ThrowIfNull(nameof(serializationAttribute));

			this.field = field;
			this.attribute = serializationAttribute;

			if ((requiredAttributeType != null) && field.IsDefined(requiredAttributeType, true))
			{
				this.requiredParameter = field.GetCustomAttribute(requiredAttributeType, true) as IRequiredParameter;
			}
			else
			{
				this.requiredParameter = null;
			}
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
