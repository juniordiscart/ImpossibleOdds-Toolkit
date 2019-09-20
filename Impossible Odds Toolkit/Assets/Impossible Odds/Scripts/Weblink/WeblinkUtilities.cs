namespace ImpossibleOdds.Weblink
{
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;

	public static class WeblinkUtilities
	{
		private static Dictionary<Type, Type> requestResponseMapping = new Dictionary<Type, Type>();

		public static Type GetResponseType(Type requestType)
		{
			requestType.ThrowIfNull(nameof(requestType));

			Type responseType = FindResponseType(requestType);

#if IMPOSSIBLE_ODDS_VERBOSE
			if (responseType == null)
			{
				Debug.LogWarningFormat("Could not find an associated response type for requests of type {0}.", requestType.Name);
			}
#endif

			return responseType;
		}

		private static Type FindResponseType(Type requestType)
		{
			if (requestResponseMapping.ContainsKey(requestType))
			{
				return requestResponseMapping[requestType];
			}

			IWeblinkResponseTypeAssociation attr = requestType.GetCustomAttributes(typeof(IWeblinkResponseTypeAssociation), false).Cast<WeblinkResponseAttribute>().FirstOrDefault();
			Type responseType = (attr != null) ? attr.ResponseType : null;
			requestResponseMapping.Add(requestType, responseType);

			return responseType;
		}
	}
}
