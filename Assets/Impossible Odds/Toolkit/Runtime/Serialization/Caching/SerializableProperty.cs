namespace ImpossibleOdds.Serialization.Caching
{
	using System;
	using System.Reflection;
	using ImpossibleOdds.ReflectionCaching;

	public class SerializableProperty : ISerializableProperty
	{
		private readonly PropertyInfo property;
		private readonly Attribute attribute;
		private readonly MethodInfo getMethod;
		private readonly MethodInfo setMethod;

		public SerializableProperty(PropertyInfo property, Attribute serializationAttribute)
		{
			property.ThrowIfNull(nameof(property));
			serializationAttribute.ThrowIfNull(nameof(serializationAttribute));

			// if (!property.CanRead || !property.CanWrite)
			// {
			// 	throw new SerializationException("The property {0} declared on type {1} does not implement the required get and set methods.", property.Name, property.DeclaringType.Name);
			// }

			this.property = property;
			this.attribute = serializationAttribute;

			if (property.CanRead)
			{
				this.getMethod = property.GetGetMethod(true);
			}

			if (property.CanWrite)
			{
				this.setMethod = property.GetSetMethod(true);
			}
		}

		/// <inheritdoc />
		public PropertyInfo Property
		{
			get => property;
		}

		/// <inheritdoc />
		public Type MemberType
		{
			get => property.PropertyType;
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
			get => property;
		}

		/// <inheritdoc />
		public object GetValue(object source)
		{
			if (getMethod != null)
			{
				return getMethod.Invoke(source, null);
			}
			else
			{
				Log.Warning("No getter has been defined for property {0} of type {1}. The value cannot retrieved. A default value is returned instead.", property.Name, property.DeclaringType.Name);
				return SerializationUtilities.GetDefaultValue(MemberType);
			}
		}

		/// <inheritdoc />
		public void SetValue(object source, object value)
		{
			if (setMethod != null)
			{
				lock (setMethod)
				{
					object[] setValueParams = TypeReflectionUtilities.GetParameterInvokationList(1);
					setValueParams[0] = value;
					setMethod.Invoke(source, setValueParams);
					TypeReflectionUtilities.ReturnParameterInvokationList(setValueParams);
				}
			}
			else
			{
				Log.Warning("No setter has been defined for property {0} of type {1}. No value will be set.", property.Name, property.DeclaringType.Name);
			}
		}
	}
}
