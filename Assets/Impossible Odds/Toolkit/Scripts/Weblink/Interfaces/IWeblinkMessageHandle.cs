namespace ImpossibleOdds.Weblink
{
	using System.Collections;

	/// <summary>
	/// Handle that keeps track of messages in transit.
	/// </summary>
	public interface IWeblinkMessageHandle : IEnumerator
	{
		/// <summary>
		/// The request of this message.
		/// </summary>
		IWeblinkRequest Request
		{
			get;
		}

		/// <summary>
		/// The response of this message.
		/// </summary>
		IWeblinkResponse Response
		{
			get;
		}

		/// <summary>
		/// True when a response has been received.
		/// </summary>
		bool IsDone
		{
			get;
		}
	}

	public interface IWeblinkMessageHandle<TRequest, TResponse> : IWeblinkMessageHandle
	where TRequest : IWeblinkRequest
	where TResponse : IWeblinkResponse
	{
		/// <inheritdoc />
		new TRequest Request
		{
			get;
		}

		/// <inheritdoc />
		new TResponse Response
		{
			get;
		}
	}
}
