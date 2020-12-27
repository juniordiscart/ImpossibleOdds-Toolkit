namespace ImpossibleOdds.Weblink
{
	using System;
	using System.Collections.Generic;

	public abstract class WeblinkMessenger<TRequest, TResponse, THandle, TResponseAssoc, TResponseCallback> : IWeblinkMessenger<TRequest, TResponse, THandle>
	where TRequest : IWeblinkRequest
	where TResponse : IWeblinkResponse
	where THandle : IWeblinkMessageHandle<TRequest, TResponse>
	where TResponseAssoc : WeblinkResponseAttribute
	where TResponseCallback : WeblinkResponseCallbackAttribute
	{
		private Dictionary<TRequest, THandle> pendingRequests = new Dictionary<TRequest, THandle>();
		private List<object> responseCallbacks = new List<object>();

		/// <inheritdoc />
		public event Action<THandle> onMessageCompleted;
		/// <inheritdoc />
		public event Action<THandle> onMessageFailed;

		/// <inheritdoc />
		event Action<IWeblinkMessageHandle> IWeblinkMessenger.onMessageCompleted
		{
			add
			{
				value.ThrowIfNull(nameof(value));
				if (value is Action<THandle> v)
				{
					onMessageCompleted += v;
				}
				else
				{
					throw new ArgumentException(nameof(value));
				}
			}
			remove
			{
				value.ThrowIfNull(nameof(value));
				if (value is Action<THandle> v)
				{
					onMessageCompleted -= v;
				}
				else
				{
					throw new ArgumentException(nameof(value));
				}
			}
		}

		//// <inheritdoc />
		event Action<IWeblinkMessageHandle> IWeblinkMessenger.onMessageFailed
		{
			add
			{
				value.ThrowIfNull(nameof(value));
				if (value is Action<THandle> v)
				{
					onMessageFailed += v;
				}
				else
				{
					throw new ArgumentException(nameof(value));
				}
			}
			remove
			{
				value.ThrowIfNull(nameof(value));
				if (value is Action<THandle> v)
				{
					onMessageFailed -= v;
				}
				else
				{
					throw new ArgumentException(nameof(value));
				}
			}
		}

		/// <inheritdoc />
		public abstract THandle SendRequest(TRequest request);

		/// <inheritdoc />
		public void RegisterCallback(object callback)
		{
			callback.ThrowIfNull(nameof(callback));
			if (!responseCallbacks.Contains(callback))
			{
				responseCallbacks.Add(callback);
			}
		}

		/// <inheritdoc />
		public void RemoveCallback(object callback)
		{
			callback.ThrowIfNull(nameof(callback));
			responseCallbacks.Remove(callback);
		}

		/// <inheritdoc />
		public THandle GetMessageHandle(TRequest request)
		{
			request.ThrowIfNull(nameof(request));

			if (!IsPending(request))
			{
				throw new WeblinkException("The request could not be found.");
			}

			return pendingRequests[request];
		}

		/// <inheritdoc />
		public bool IsPending(TRequest request)
		{
			request.ThrowIfNull(nameof(request));
			return pendingRequests.ContainsKey(request);
		}

		/// <inheritdoc />
		public virtual void Stop(TRequest request)
		{
			request.ThrowIfNull(nameof(request));

			if (!IsPending(request))
			{
				throw new WeblinkException("The request could not be found.");
			}

			pendingRequests.Remove(request);
		}

		/// <inheritdoc />
		public virtual void StopAll()
		{
			pendingRequests.Clear();
		}

		/// Adds the handle to the set of requests still pending a response.
		protected virtual void AddPendingRequest(THandle handle)
		{
			handle.ThrowIfNull(nameof(handle));
			handle.Request.ThrowIfNull(nameof(handle.Request));
			if (IsPending(handle.Request))
			{
				throw new WeblinkException("A handle with the same request is already pending.");
			}

			pendingRequests.Add(handle.Request, handle);
		}

		/// Removes the handle from the set of pending requests.
		protected virtual void RemovePendingRequest(THandle handle)
		{
			handle.ThrowIfNull(nameof(handle));
			handle.Request.ThrowIfNull(nameof(handle.Request));
			pendingRequests.Remove(handle.Request);
		}

		/// Notifies interested parties the message is completed.
		protected void HandleCompleted(THandle handle)
		{
			handle.ThrowIfNull(nameof(handle));
			InvokeResponseCallbacks(handle);
			onMessageCompleted.InvokeIfNotNull(handle);
		}

		/// Notifies interested parties the message had an error.
		protected virtual void HandleFailed(THandle handle)
		{
			handle.ThrowIfNull(nameof(handle));
			InvokeResponseCallbacks(handle);
			onMessageFailed.InvokeIfNotNull(handle);
		}

		/// Creates a response instance of type associated with the request.
		protected virtual TResponse InstantiateResponse(THandle handle)
		{
			handle.ThrowIfNull(nameof(handle));
			Type responseType = WeblinkUtilities.GetResponseType<TResponseAssoc>(handle.Request.GetType());

			if (responseType == null)
			{
				throw new WeblinkException("Could not determine the expected type of response for requests of type '{0}'.", handle.Request.GetType().Name);
			}
			else if (responseType.IsAbstract || responseType.IsInterface)
			{
				throw new WeblinkException("Cannot create a response instance of type '{0}' because it is abstract or an interface.", responseType.Name);
			}

			// Instantiate a response
			return (TResponse)Activator.CreateInstance(responseType);
		}

		/// Invokes callback methods on registered callback objects.
		protected virtual void InvokeResponseCallbacks(THandle handle)
		{
			handle.ThrowIfNull(nameof(handle));
			foreach (object callback in responseCallbacks)
			{
				if (callback != null)
				{
					WeblinkUtilities.InvokeResponseCallback<TResponseCallback>(callback, handle);
				}
			}
		}

		IWeblinkMessageHandle IWeblinkMessenger.GetMessageHandle(IWeblinkRequest request)
		{
			request.ThrowIfNull(nameof(request));
			if (request is TRequest r)
			{
				return GetMessageHandle(r);
			}

			throw new ArgumentException(nameof(request));
		}

		bool IWeblinkMessenger.IsPending(IWeblinkRequest request)
		{
			request.ThrowIfNull(nameof(request));
			if (request is TRequest r)
			{
				return IsPending(r);
			}

			throw new ArgumentException(nameof(request));
		}

		IWeblinkMessageHandle IWeblinkMessenger.SendRequest(IWeblinkRequest request)
		{
			request.ThrowIfNull(nameof(request));
			if (request is TRequest r)
			{
				return SendRequest(r);
			}

			throw new ArgumentException(nameof(request));
		}

		void IWeblinkMessenger.Stop(IWeblinkRequest request)
		{
			request.ThrowIfNull(nameof(request));
			if (request is TRequest r)
			{
				Stop(r);
			}

			throw new ArgumentException(nameof(request));
		}
	}
}
