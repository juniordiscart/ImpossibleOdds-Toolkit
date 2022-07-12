namespace ImpossibleOdds.Serialization.Caching
{
	using System;
	using System.Reflection;
	using ImpossibleOdds;
	using ImpossibleOdds.ReflectionCaching;

	public class SerializationCallbackMethod : ISerializationCallback
	{
		private readonly MethodInfo callback;
		private readonly Attribute attribute;
		private readonly ParameterInfo[] parameters;

		public SerializationCallbackMethod(MethodInfo callbackMethod, Attribute callbackAttribute)
		{
			callbackMethod.ThrowIfNull(nameof(callbackMethod));
			callbackAttribute.ThrowIfNull(nameof(callbackAttribute));

			this.callback = callbackMethod;
			this.attribute = callbackAttribute;
			this.parameters = callback.GetParameters();
		}

		/// <inheritdoc />
		public MethodInfo Method
		{
			get => callback;
		}

		/// <inheritdoc />
		public ParameterInfo[] Parameters
		{
			get => parameters;
		}

		/// <inheritdoc />
		MemberInfo IMemberAttributePair.Member
		{
			get => callback;
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
		public void InvokeCallback(object source, params object[] args)
		{
			callback.Invoke(source, args);
		}
	}
}
