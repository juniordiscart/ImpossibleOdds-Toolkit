using System;
using System.Reflection;
using ImpossibleOdds.ReflectionCaching;

namespace ImpossibleOdds.Serialization.Caching
{
	public class SerializableProperty : ISerializableProperty
	{
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

			Property = property;
			Attribute = serializationAttribute;

			if (property.CanRead)
			{
				getMethod = property.GetGetMethod(true);
			}

			if (property.CanWrite)
			{
				setMethod = property.GetSetMethod(true);
			}
		}

		/// <inheritdoc />
		public PropertyInfo Property { get; }

		/// <inheritdoc />
		public Type MemberType => Property.PropertyType;

		/// <inheritdoc />
		public Attribute Attribute { get; }

		/// <inheritdoc />
		public Type AttributeType => Attribute.GetType();

		/// <inheritdoc />
		MemberInfo IMemberAttributePair.Member => Property;

		/// <inheritdoc />
		public object GetValue(object source)
		{
			if (getMethod != null)
			{
				return getMethod.Invoke(source, null);
			}

			Log.Warning("No getter has been defined for property {0} of type {1}. The value cannot retrieved. A default value is returned instead.", Property.Name, Property.DeclaringType.Name);
			return SerializationUtilities.GetDefaultValue(MemberType);
		}

		/// <inheritdoc />
		public void SetValue(object source, object value)
		{
			if (setMethod != null)
			{
				lock (setMethod)
				{
					object[] setValueParams = TypeReflectionUtilities.GetParameterInvocationList(1);
					setValueParams[0] = value;
					setMethod.Invoke(source, setValueParams);
					TypeReflectionUtilities.ReturnParameterInvocationList(setValueParams);
				}
			}
			else
			{
				Log.Warning("No setter has been defined for property {0} of type {1}. No value will be set.", Property.Name, Property.DeclaringType.Name);
			}
		}
	}
}