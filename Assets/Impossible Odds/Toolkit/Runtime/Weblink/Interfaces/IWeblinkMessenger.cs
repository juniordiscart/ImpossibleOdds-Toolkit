namespace ImpossibleOdds.Weblink
{
	using System;

	/// <summary>
	/// A messenger interface for managing outgoing requests and incoming responses.
	/// </summary>
	public interface IWeblinkMessenger
	{
		event Action<IWeblinkMessageHandle> onMessageCompleted;
		event Action<IWeblinkMessageHandle> onMessageFailed;

		/// <summary>
		/// Is the request still waiting for a response?
		/// </summary>
		/// <param name="request">The request to check.</param>
		/// <returns>True when waiting for a response, false otherwise.</returns>
		bool IsPending(IWeblinkRequest request);

		/// <summary>
		/// If the request is pending, it will be requested to cancel the process.
		/// </summary>
		/// <param name="request">The request to cancel.</param>
		void Stop(IWeblinkRequest request);

		/// <summary>
		/// Stop all pending requests.
		/// </summary>
		void StopAll();

		/// <summary>
		/// Get the message handle for the pending request.
		/// </summary>
		/// <param name="request">The request for which to get the message handle.</param>
		/// <returns>The message handle for the pending request, null otherwise.</returns>
		IWeblinkMessageHandle GetMessageHandle(IWeblinkRequest request);

		/// <summary>
		/// Send the request and let it wait for a response.
		/// </summary>
		/// <param name="request">The request to send out.</param>
		/// <returns>A message handle which will receive the response when available.</returns>
		IWeblinkMessageHandle SendRequest(IWeblinkRequest request);

		/// <summary>
		/// Register an object that is interested in receiving a notification when a message handle is done.
		/// </summary>
		/// <param name="callback">The object interested in receiving a callback.</param>
		void RegisterCallback(object callback);

		/// <summary>
		/// Remove an object that was interested in receiving callbacks about message handles.
		/// </summary>
		/// <param name="callback">The object to remove from the callback set.</param>
		void RemoveCallback(object callback);
	}

	/// <summary>
	/// Generic variant that provides thighther control over the types that can be used.
	/// </summary>
	/// <typeparam name="TRequest">Type of requests that will get sent.</typeparam>
	/// <typeparam name="TResponse">Type of response that will be received.</typeparam>
	/// <typeparam name="TMessageHandle">Type of the handle per request-response pair.</typeparam>
	public interface IWeblinkMessenger<TRequest, TResponse, TMessageHandle> : IWeblinkMessenger
	where TRequest : IWeblinkRequest
	where TResponse : IWeblinkResponse
	where TMessageHandle : IWeblinkMessageHandle<TRequest, TResponse>
	{
		new event Action<TMessageHandle> onMessageCompleted;
		new event Action<TMessageHandle> onMessageFailed;

		/// <summary>
		/// Is the request still waiting for a response?
		/// </summary>
		/// <param name="request">The request to check.</param>
		/// <returns>True when waiting for a response, false otherwise.</returns>
		bool IsPending(TRequest request);

		/// <summary>
		/// If the request is pending, it will be requested to cancel the process.
		/// </summary>
		/// <param name="request">The request to cancel.</param>
		void Stop(TRequest request);

		/// <summary>
		/// Get the message handle for the pending request.
		/// </summary>
		/// <param name="request">The request for which to get the message handle.</param>
		/// <returns>The message handle for the pending request, null otherwise.</returns>
		TMessageHandle GetMessageHandle(TRequest request);

		/// <summary>
		/// Send the request and let it wait for a response.
		/// </summary>
		/// <param name="request">The request to send out.</param>
		/// <returns>A message handle which will receive the response when available.</returns>
		TMessageHandle SendRequest(TRequest request);
	}
}
