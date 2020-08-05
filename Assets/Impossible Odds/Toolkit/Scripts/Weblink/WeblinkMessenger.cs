namespace ImpossibleOdds.Weblink
{
	using System;
	using System.Collections.Generic;

	public abstract class WeblinkMessenger<TRequest, TResponse, TResponseAssociation>
	where TRequest : IWeblinkRequest
	where TResponse : IWeblinkResponse
	where TResponseAssociation : Attribute, IWeblinkResponseTypeAssociation
	{
		public event Action<TRequest, TResponse> onResponseReceived;

		private List<TRequest> pendingRequests = new List<TRequest>();

		public bool IsPendingRequest(TRequest request)
		{
			request.ThrowIfNull(nameof(request));
			return pendingRequests.Contains(request);
		}

		public void ClearRequest(TRequest request)
		{
			request.ThrowIfNull(nameof(request));
			pendingRequests.Remove(request);
		}

		public void ClearAllPendingRequests()
		{
			pendingRequests.Clear();
		}

		public void SendRequest(TRequest request)
		{
			request.ThrowIfNull(nameof(request));

			if (IsPendingRequest(request))
			{
				string errMsg = "This request is already sent.";
				Debug.Error(errMsg);
				throw new WeblinkException(errMsg);
			}

			if (SendRequestData(request))
			{
				pendingRequests.Add(request);
			}
			else
			{
				string errMsg = string.Format("Failed to send request of type {0}.", request.GetType().Name);
				Debug.Error(errMsg);
				throw new WeblinkException(errMsg);
			}
		}

		public void ReceiveResponseData(object responseData)
		{
			responseData.ThrowIfNull(nameof(responseData));

			for (int i = 0; i < pendingRequests.Count;)
			{
				if (pendingRequests[i].IsResponseData(responseData))
				{
					TRequest request = pendingRequests[i];
					TResponse response = InstantiateResponse(request.GetType());
					ProcessResponseData(request, response, responseData);
					pendingRequests.RemoveAt(i);

					if (onResponseReceived != null)
					{
						onResponseReceived(request, response);
					}
				}
				else
				{
					++i;
				}
			}
		}

		private TResponse InstantiateResponse(Type requestType)
		{
			Type responseType = WeblinkUtilities.GetResponseType(requestType);
			if (responseType == null)
			{
				string errMsg = string.Format("Could not determine the response type associated with requests of type {0}.", requestType.Name);
				Debug.Error(errMsg);
				throw new WeblinkException(errMsg);
			}
			else if (responseType.IsAbstract || responseType.IsInterface)
			{
				string errMsg = string.Format("Cannot create a response instance of type {0} because it is abstract or is an interface.", responseType.Name);
				Debug.Error(errMsg);
				throw new WeblinkException(errMsg);
			}

			// Instantiate a response
			return (TResponse)Activator.CreateInstance(responseType);
		}

		protected abstract bool SendRequestData(TRequest request);
		protected abstract void ProcessResponseData(TRequest request, TResponse response, object responseData);
	}
}
