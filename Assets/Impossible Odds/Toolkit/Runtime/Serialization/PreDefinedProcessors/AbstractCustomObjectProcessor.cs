using System;
using System.Reflection;
using ImpossibleOdds.ReflectionCaching;
using ImpossibleOdds.Serialization.Caching;

namespace ImpossibleOdds.Serialization.Processors
{
	public abstract class AbstractCustomObjectProcessor : ISerializationProcessor, IDeserializationProcessor
	{
		public AbstractCustomObjectProcessor(ISerializationDefinition definition)
		{
			Definition = definition;
		}

		/// <inheritdoc />
		public ISerializationDefinition Definition { get; }

		/// <summary>
		/// Does the serialization definition supports callbacks?
		/// </summary>
		public bool SupportsSerializationCallbacks => CallbackFeature != null;

		/// <summary>
		/// The callbacks support feature of the current serialization definition.
		/// </summary>
		public ICallbackFeature CallbackFeature { get; set; }

		/// <inheritdoc />
		public abstract object Serialize(object objectToSerialize);

		/// <inheritdoc />
		public abstract object Deserialize(Type targetType, object dataToDeserialize);

		/// <inheritdoc />
		public abstract bool CanSerialize(object objectToSerialize);

		/// <inheritdoc />
		public abstract bool CanDeserialize(Type targetType, object dataToDeserialize);

		/// <summary>
		/// Let the target object know that serialization will start.
		/// </summary>
		/// <param name="target">Target object to notify.</param>
		protected void InvokeOnSerializationCallback(object target)
		{
			if (SupportsSerializationCallbacks)
			{
				InvokeCallback(target, CallbackFeature.OnSerializationAttribute);
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
				InvokeCallback(target, CallbackFeature.OnSerializedAttribute);
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
				InvokeCallback(target, CallbackFeature.OnDeserializationAttribute);
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
				InvokeCallback(target, CallbackFeature.OnDeserializedAttribute);
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
					object[] callbackParams = TypeReflectionUtilities.GetParameterInvocationList(callbackParameters.Length);
					for (int i = 0; i < callbackParameters.Length; ++i)
					{
						Type parameterType = callbackParameters[i].ParameterType;
						callbackParams[i] =
							parameterType.IsAssignableFrom(processorType) ?
							this :
							SerializationUtilities.GetDefaultValue(parameterType);
					}

					callback.Method.Invoke(target, callbackParameters);
					TypeReflectionUtilities.ReturnParameterInvocationList(callbackParams);
				}
				else
				{
					callback.Method.Invoke(target, null);
				}
			}
		}
	}
}