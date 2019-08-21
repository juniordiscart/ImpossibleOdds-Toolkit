namespace ImpossibleOdds.Weblink
{
	using System;
	using System.Reflection;
	using System.Collections.Generic;

	using UnityEngine;

	public abstract class WeblinkMessenger<TRequest, TResponse, TResponseAssociation>
	where TRequest : IWeblinkRequest
	where TResponse : IWeblinkResponse
	where TResponseAssociation : Attribute, IWeblinkResponseTypeAssociation
	{
		public event Action<TRequest, TResponse> onResponseReceived;

		private List<TRequest> pendingRequests = new List<TRequest>();

		public bool IsPendingRequest(TRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}

			return pendingRequests.Contains(request);
		}

		public void ClearRequest(TRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}

			pendingRequests.Remove(request);
		}

		public void ClearAllPendingRequests()
		{
			pendingRequests.Clear();
		}

		public void SendRequest(TRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}

			if (IsPendingRequest(request))
			{
				string errMsg = "This request is already sent.";
#if IMPOSSIBLE_ODDS_VERBOSE
				Debug.LogError(errMsg);
#endif
				throw new WeblinkException(errMsg);
			}

			if (SendRequestData(request))
			{
				pendingRequests.Add(request);
			}
			else
			{
				string errMsg = string.Format("Failed to send request of type {0}.", request.GetType().Name);
#if IMPOSSIBLE_ODDS_VERBOSE
				Debug.LogError(errMsg);
#endif
				throw new WeblinkException(errMsg);
			}
		}

		public void ReceiveResponseData(object responseData)
		{
			if (responseData == null)
			{
				throw new ArgumentNullException("responseData");
			}

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
#if IMPOSSIBLE_ODDS_VERBOSE
				Debug.LogError(errMsg);
#endif
				throw new WeblinkException(errMsg);
			}
			else if (responseType.IsAbstract || responseType.IsInterface)
			{
				string errMsg = string.Format("Cannot create a response instance of type {0} because it is abstract or is an interface.", responseType.Name);
#if IMPOSSIBLE_ODDS_VERBOSE
				Debug.LogError(errMsg);
#endif
				throw new WeblinkException(errMsg);
			}

			// Instantiate using a default constructor, else we use the activator
			ConstructorInfo c = responseType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
			if (c != null)
			{
				return (TResponse)c.Invoke(null);
			}
			else
			{
				return (TResponse)Activator.CreateInstance(responseType);
			}
		}

		protected abstract bool SendRequestData(TRequest request);
		protected abstract void ProcessResponseData(TRequest request, TResponse response, object responseData);
	}
}
