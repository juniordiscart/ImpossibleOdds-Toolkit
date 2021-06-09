namespace ImpossibleOdds.Serialization.Caching
{
	using System;
	using System.Reflection;
	using ImpossibleOdds.Serialization.Processors;

	public struct SerializationCallbackInfo
	{
		/// <summary>
		/// The callback method.
		/// </summary>
		public readonly MethodInfo callbackMethod;
		/// <summary>
		/// The info about the parameters.
		/// </summary>
		public readonly ParameterInfo[] parameters;
		/// <summary>
		/// Cached list of parameter list that can be invoked.
		/// </summary>
		public readonly object[] parameterInvokation;

		public SerializationCallbackInfo(MethodInfo methodInfo)
		{
			methodInfo.ThrowIfNull(nameof(methodInfo));
			this.callbackMethod = methodInfo;
			this.parameters = methodInfo.GetParameters();
			this.parameterInvokation = (this.parameters.Length > 0) ? new object[this.parameters.Length] : null;
		}

		/// <summary>
		/// Invokes the callback on the given target.
		/// </summary>
		/// <param name="target">The target object unto which the callback is invoked.</param>
		/// <param name="processor">The processor currently dealing with the object.</param>
		public void Invoke(object target, IProcessor processor = null)
		{
			target.ThrowIfNull(nameof(target));

			if ((processor == null) || (parameters.Length == 0))
			{
				callbackMethod.Invoke(target, null);
			}
			else
			{
				Type processorType = processor.GetType();
				for (int i = 0; i < parameters.Length; ++i)
				{
					parameterInvokation[i] = parameters[i].ParameterType.IsAssignableFrom(processorType) ? processor : null;
				}

				callbackMethod.Invoke(target, parameterInvokation);
			}
		}
	}
}
