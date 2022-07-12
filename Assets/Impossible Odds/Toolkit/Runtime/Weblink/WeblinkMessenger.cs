namespace ImpossibleOdds.Weblink
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Concurrent;

	/// <summary>
	/// An abstract messenger framework for matching outgoing requests with a single incoming response.
	/// </summary>
	/// <typeparam name="TRequest">Type of requests.</typeparam>
	/// <typeparam name="TResponse">Type of responeses.</typeparam>
	/// <typeparam name="THandle">Type of handles.</typeparam>
	/// <typeparam name="TResponseAssoc">Type of the attribute that associates a request type with a response type.</typeparam>
	/// <typeparam name="TResponseCallback">Type of the attribute for invoking targeted callbacks.</typeparam>
	public abstract class WeblinkMessenger<TRequest, TResponse, THandle, TResponseAssoc, TResponseCallback> : IWeblinkMessenger<TRequest, TResponse, THandle>
	where TRequest : IWeblinkRequest
	where TResponse : IWeblinkResponse
	where THandle : IWeblinkMessageHandle<TRequest, TResponse>
	where TResponseAssoc : WeblinkResponseAttribute
	where TResponseCallback : WeblinkResponseCallbackAttribute
	{
		/// <inheritdoc />
		public event Action<THandle> onMessageCompleted;
		/// <inheritdoc />
		public event Action<THandle> onMessageFailed;

		private ConcurrentDictionary<TRequest, THandle> pendingRequests = new ConcurrentDictionary<TRequest, THandle>();
		private List<object> responseCallbacks = new List<object>();

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

		/// <inheritdoc />
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

		/// <summary>
		/// The amount of pending registered requests.
		/// </summary>
		public int Count
		{
			get => pendingRequests.Count;
		}

		/// <inheritdoc />
		public abstract THandle SendRequest(TRequest request);

		/// <inheritdoc />
		public void RegisterCallback(object callback)
		{
			callback.ThrowIfNull(nameof(callback));
			lock (responseCallbacks)
			{
				if (!responseCallbacks.Contains(callback))
				{
					responseCallbacks.Add(callback);
				}
			}
		}

		/// <inheritdoc />
		public void RemoveCallback(object callback)
		{
			callback.ThrowIfNull(nameof(callback));
			lock (responseCallbacks)
			{
				responseCallbacks.Remove(callback);
			}
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

			if (IsPending(request))
			{
				RemovePendingRequest(request);
			}
		}

		/// <inheritdoc />
		public virtual void StopAll()
		{
			pendingRequests.Clear();
		}

		/// <summary>
		/// Adds the handle to the set of requests still pending a response.
		/// </summary>
		/// <param name="handle">The handle to add to the set of pending requests.</param>
		protected virtual void AddPendingRequest(THandle handle)
		{
			handle.ThrowIfNull(nameof(handle));
			handle.Request.ThrowIfNull(nameof(handle.Request));
			if (IsPending(handle.Request))
			{
				throw new WeblinkException("A handle with the same request is already pending.");
			}

			pendingRequests.TryAdd(handle.Request, handle);
		}

		/// <summary>
		/// Removes the handle from the set of pending requests.
		/// </summary>
		/// <param name="handle">The handle to remove from the pending requests.</param>
		protected virtual void RemovePendingRequest(THandle handle)
		{
			handle.ThrowIfNull(nameof(handle));
			RemovePendingRequest(handle.Request);
		}

		/// <summary>
		/// Removes the pending handle associated with the given request.
		/// </summary>
		/// <param name="request">The request associated with the handle to be removed.</param>
		protected virtual void RemovePendingRequest(TRequest request)
		{
			request.ThrowIfNull(nameof(request));
			pendingRequests.TryRemove(request, out _);
		}

		/// <summary>
		/// Notifies interested parties the message is completed.
		/// </summary>
		/// <param name="handle">The handle that is completed.</param>
		protected void HandleCompleted(THandle handle)
		{
			handle.ThrowIfNull(nameof(handle));
			InvokeResponseCallbacks(handle);
			onMessageCompleted.InvokeIfNotNull(handle);
		}

		/// <summary>
		/// Notifies interested parties the message had an error.
		/// </summary>
		/// <param name="handle">The handle that failed.</param>
		protected virtual void HandleFailed(THandle handle)
		{
			handle.ThrowIfNull(nameof(handle));
			InvokeResponseCallbacks(handle);
			onMessageFailed.InvokeIfNotNull(handle);
		}

		/// <summary>
		/// Creates a response instance of type associated with the request.
		/// </summary>
		/// <param name="handle">The handle for which a response should be instantiated.</param>
		/// <returns>A matching response object that can be used to apply the response values to.</returns>
		protected virtual TResponse InstantiateResponse(THandle handle)
		{
			handle.ThrowIfNull(nameof(handle));

			if (!WeblinkUtilities.IsResponseTypeDefined<TResponseAssoc>(handle.Request.GetType()))
			{
				throw new WeblinkException("The request of type {0} does not have a response type defined using {1}.", handle.Request.GetType().Name, typeof(TResponseAssoc).Name);
			}

			Type responseType = WeblinkUtilities.GetResponseType<TResponseAssoc>(handle.Request.GetType());

			if (responseType.IsAbstract || responseType.IsInterface)
			{
				throw new WeblinkException("Cannot create a response instance of type {0} because it is abstract or an interface.", responseType.Name);
			}

			// Instantiate a response
			return (TResponse)Activator.CreateInstance(responseType, true);
		}

		/// <summary>
		/// Invokes callback methods on registered callback objects.
		/// </summary>
		/// <param name="handle">The handle for which to invoke registered callbs.</param>
		protected virtual void InvokeResponseCallbacks(THandle handle)
		{
			handle.ThrowIfNull(nameof(handle));
			foreach (object callback in responseCallbacks)
			{
				if (callback != null)
				{
					WeblinkUtilities.InvokeResponseCallback<TResponseCallback, TResponseAssoc>(callback, handle);
				}
			}
		}

		/// <inheritdoc />
		IWeblinkMessageHandle IWeblinkMessenger.GetMessageHandle(IWeblinkRequest request)
		{
			request.ThrowIfNull(nameof(request));
			if (request is TRequest r)
			{
				return GetMessageHandle(r);
			}

			throw new ArgumentException(nameof(request));
		}

		/// <inheritdoc />
		bool IWeblinkMessenger.IsPending(IWeblinkRequest request)
		{
			request.ThrowIfNull(nameof(request));
			if (request is TRequest r)
			{
				return IsPending(r);
			}

			throw new ArgumentException(nameof(request));
		}

		/// <inheritdoc />
		IWeblinkMessageHandle IWeblinkMessenger.SendRequest(IWeblinkRequest request)
		{
			request.ThrowIfNull(nameof(request));
			if (request is TRequest r)
			{
				return SendRequest(r);
			}

			throw new ArgumentException(nameof(request));
		}

		/// <inheritdoc />
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
