using System;
using System.Reflection;
using ImpossibleOdds.ReflectionCaching;

namespace ImpossibleOdds.Serialization.Caching
{
	public class SerializationCallbackMethod : ISerializationCallback
	{
		public SerializationCallbackMethod(MethodInfo callbackMethod, Attribute callbackAttribute)
		{
			callbackMethod.ThrowIfNull(nameof(callbackMethod));
			callbackAttribute.ThrowIfNull(nameof(callbackAttribute));

			Method = callbackMethod;
			Attribute = callbackAttribute;
			Parameters = Method.GetParameters();
		}

		/// <inheritdoc />
		public MethodInfo Method { get; }

		/// <inheritdoc />
		public ParameterInfo[] Parameters { get; }

		/// <inheritdoc />
		MemberInfo IMemberAttributePair.Member => Method;

		/// <inheritdoc />
		public Attribute Attribute { get; }

		/// <inheritdoc />
		public Type AttributeType => Attribute.GetType();

		/// <inheritdoc />
		public void InvokeCallback(object source, params object[] args)
		{
			Method.Invoke(source, args);
		}
	}
}