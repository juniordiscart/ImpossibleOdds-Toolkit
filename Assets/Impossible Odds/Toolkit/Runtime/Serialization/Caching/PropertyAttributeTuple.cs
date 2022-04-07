namespace ImpossibleOdds.Serialization.Caching
{
	using System;
	using System.Reflection;

	public struct PropertyAttributeTuple : IMemberAttributeTuple<PropertyInfo>
	{
		private readonly PropertyInfo property;
		private readonly Attribute attribute;
		private readonly MethodInfo getMethod;
		private readonly MethodInfo setMethod;

		/// <summary>
		/// The registered property.
		/// </summary>
		public PropertyInfo Property
		{
			get { return property; }
		}

		/// <inheritdoc />
		public Type MemberType
		{
			get { return property.PropertyType; }
		}

		/// <inheritdoc />
		public PropertyInfo Member
		{
			get { return property; }
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

		public PropertyAttributeTuple(PropertyInfo property, Attribute serializationAttribute)
		{
			property.ThrowIfNull(nameof(property));
			serializationAttribute.ThrowIfNull(nameof(serializationAttribute));

			this.property = property;
			this.attribute = serializationAttribute;

			this.getMethod = property.GetGetMethod(true);
			this.setMethod = property.GetSetMethod(true);
		}

		/// <inheritdoc />
		public object GetValue(object source)
		{
			if (getMethod == null)
			{
				throw new SerializationException("The property {0} declared on type {1} does not implement a 'get'-method.", property.Name, property.DeclaringType.Name);
			}

			return getMethod.Invoke(source, null);
		}

		/// <inheritdoc />
		public void SetValue(object source, object value)
		{
			if (setMethod == null)
			{
				throw new SerializationException("The property {0} declared on type {1} does not implement a 'set'-method.", property.Name, property.DeclaringType.Name);
			}

			setMethod.Invoke(source, new object[] { value });
		}
	}
}
