namespace ImpossibleOdds.Weblink.Caching
{
	using System;
	using System.Reflection;
	using ImpossibleOdds.ReflectionCaching;

	public class WeblinkTargetedCallbackMethod : ITargetedCallback
	{
		private readonly MethodInfo callback;
		private readonly Attribute attribute;
		private readonly Type responseType;
		private readonly ParameterInfo[] parameters;

		public WeblinkTargetedCallbackMethod(MethodInfo callbackMethod, Attribute callbackAttribute)
		{
			callbackMethod.ThrowIfNull(nameof(callbackMethod));
			callbackAttribute.ThrowIfNull(nameof(callbackAttribute));

			if (callbackAttribute is IWeblinkResponseTypeAssociation responseAssociation)
			{
				this.responseType = responseAssociation.ResponseType;
			}
			else
			{
				throw new ReflectionCachingException("The attribute defining a targeted callback does not implement the {0} interface.", callbackAttribute.GetType().Name);
			}

			this.callback = callbackMethod;
			this.attribute = callbackAttribute;
			this.parameters = callbackMethod.GetParameters();
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
		public Type ResponseType
		{
			get => responseType;
		}
	}
}
