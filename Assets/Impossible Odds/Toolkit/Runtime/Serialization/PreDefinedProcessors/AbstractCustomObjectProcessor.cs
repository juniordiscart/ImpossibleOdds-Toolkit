namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections.Generic;
	using ImpossibleOdds.Serialization.Caching;

	public abstract class AbstractCustomObjectProcessor : ISerializationProcessor, IDeserializationProcessor
	{
		private ISerializationDefinition definition = null;
		private ICallbacksSupport callbacksDefinition = null;

		/// <inheritdoc />
		public ISerializationDefinition Definition
		{
			get { return definition; }
		}

		/// <summary>
		/// Does the serialization definition supports callbacks?
		/// </summary>
		public bool SupportsSerializationCallbacks
		{
			get { return callbacksDefinition != null; }
		}

		/// <summary>
		/// The callbacks support feature of the current serialization definition.
		/// </summary>
		public ICallbacksSupport CallbacksSupport
		{
			get { return callbacksDefinition; }
		}

		public AbstractCustomObjectProcessor(ISerializationDefinition definition)
		{
			this.definition = definition;
			this.callbacksDefinition = (definition is ICallbacksSupport) ? definition as ICallbacksSupport : null;
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

			IEnumerable<SerializationCallbackInfo> callbacks = SerializationUtilities.GetTypeMap(target.GetType()).GetSerializationCallbacks(callbackAttributeType);
			foreach (SerializationCallbackInfo callback in callbacks)
			{
				callback.Invoke(target, this);
			}
		}
	}
}
