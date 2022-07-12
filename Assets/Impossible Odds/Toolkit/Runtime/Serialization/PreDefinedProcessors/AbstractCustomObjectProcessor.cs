namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Reflection;
	using ImpossibleOdds.ReflectionCaching;
	using ImpossibleOdds.Serialization.Caching;

	public abstract class AbstractCustomObjectProcessor : ISerializationProcessor, IDeserializationProcessor
	{
		private readonly ISerializationDefinition definition = null;
		private readonly ICallbacksSupport callbacksDefinition = null;

		public AbstractCustomObjectProcessor(ISerializationDefinition definition)
		{
			this.definition = definition;
			this.callbacksDefinition = (definition is ICallbacksSupport) ? definition as ICallbacksSupport : null;
		}

		/// <inheritdoc />
		public ISerializationDefinition Definition
		{
			get => definition;
		}

		/// <summary>
		/// Does the serialization definition supports callbacks?
		/// </summary>
		public bool SupportsSerializationCallbacks
		{
			get => callbacksDefinition != null;
		}

		/// <summary>
		/// The callbacks support feature of the current serialization definition.
		/// </summary>
		public ICallbacksSupport CallbacksSupport
		{
			get => callbacksDefinition;
		}

		/// <inheritdoc />
		public abstract bool Serialize(object objectToSerialize, out object serializedResult);

		/// <inheritdoc />
		public abstract bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult);

		/// <summary>
		/// Let the target object know that serialization will start.
		/// </summary>
		/// <param name="target">Target object to notify.</param>
		protected void InvokeOnSerializationCallback(object target)
		{
			if (SupportsSerializationCallbacks)
			{
				InvokeCallback(target, CallbacksSupport.OnSerializationCallbackType);
			}
		}

		/// <summary>
		/// Let the target object know that serialization has ended.
		/// </summary>
		/// <param name="target">Target object to notify.</param>
		protected void InvokeOnSerializedCallback(object target)
		{
			if (SupportsSerializationCallbacks)
			{
				InvokeCallback(target, CallbacksSupport.OnSerializedCallbackType);
			}
		}

		/// <summary>
		/// Let the target object know that deserialization will start.
		/// </summary>
		/// <param name="target">Target object to notify.</param>
		protected void InvokeOnDeserializationCallback(object target)
		{
			if (SupportsSerializationCallbacks)
			{
				InvokeCallback(target, CallbacksSupport.OnDeserializionCallbackType);
			}
		}

		/// <summary>
		/// Let the target object know that deserialization has ended.
		/// </summary>
		/// <param name="target">Target object to notify.</param>
		protected void InvokeOnDeserializedCallback(object target)
		{
			if (SupportsSerializationCallbacks)
			{
				InvokeCallback(target, CallbacksSupport.OnDeserializedCallbackType);
			}
		}

		private void InvokeCallback(object target, Type callbackAttributeType)
		{
			target.ThrowIfNull(nameof(target));

			if (callbackAttributeType == null)
			{
				return;
			}

			ISerializationCallback[] callbacks = SerializationUtilities.GetTypeMap(target.GetType()).GetSerializationCallbackMethods(callbackAttributeType);
			foreach (ISerializationCallback callback in callbacks)
			{
				if (callback.Parameters.Length > 0)
				{
					Type processorType = this.GetType();
					ParameterInfo[] callbackParameters = callback.Parameters;
					object[] callbackParams = TypeReflectionUtilities.GetParameterInvokationList(callbackParameters.Length);
					for (int i = 0; i < callbackParameters.Length; ++i)
					{
						Type parameterType = callbackParameters[i].ParameterType;
						callbackParams[i] =
							parameterType.IsAssignableFrom(processorType) ?
							this :
							SerializationUtilities.GetDefaultValue(parameterType);
					}

					callback.Method.Invoke(target, callbackParameters);
					TypeReflectionUtilities.ReturnParameterInvokationList(callbackParams);
				}
				else
				{
					callback.Method.Invoke(target, null);
				}
			}
		}
	}
}
