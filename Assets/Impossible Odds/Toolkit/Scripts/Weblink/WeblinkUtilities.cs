namespace ImpossibleOdds.Weblink
{
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using System.Reflection;

	public static class WeblinkUtilities
	{
		private readonly static Type ObjectType = typeof(object);
		private readonly static Type HandleType = typeof(IWeblinkMessageHandle);
		private readonly static Type RequestType = typeof(IWeblinkRequest);
		private readonly static Type ResponseType = typeof(IWeblinkResponse);

		private readonly static Dictionary<Type, Type> requestResponseMapping = new Dictionary<Type, Type>();

		/// <summary>
		/// Retrieve the associated response type for the given request type.
		/// </summary>
		/// <param name="requestType">The type of the request for which to find the type of response.</param>
		/// <typeparam name="TResponseAssocAttr">Type of attribute to look for on the request type.</typeparam>
		/// <returns>The type of the associated response for the type of request.</returns>
		public static Type GetResponseType<TResponseAssocAttr>(Type requestType)
		where TResponseAssocAttr : WeblinkResponseAttribute
		{
			requestType.ThrowIfNull(nameof(requestType));

			if (!requestResponseMapping.ContainsKey(requestType))
			{
				TResponseAssocAttr attr = requestType.GetCustomAttribute<TResponseAssocAttr>(false);
				requestResponseMapping.Add(requestType, attr?.ResponseType);
			}

			return requestResponseMapping[requestType];
		}

		/// <summary>
		/// Retrieve the associated response type for the given request type.
		/// </summary>
		/// <typeparam name="TRequestType">The type of the request for which to find the type of response.</typeparam>
		/// <typeparam name="TResponseAssocAttr">Type of attribute to look for on the request type.</typeparam>
		/// <returns>The type of the associated response for the type of request.</returns>
		public static Type GetResponseType<TRequestType, TResponseAssocAttr>()
		where TRequestType : IWeblinkRequest
		where TResponseAssocAttr : WeblinkResponseAttribute
		{
			return GetResponseType<TResponseAssocAttr>(typeof(TRequestType));
		}

		/// <summary>
		/// Checks whether a response type is defined for the given request type.
		/// </summary>
		/// <param name="requestType">The type of request for which to check if a type of response is defined.</param>
		/// <typeparam name="TResponseAssocAttr"></typeparam>
		/// <returns>True, if the request type has defined a response type.</returns>
		public static bool IsResponseTypeDefined<TResponseAssocAttr>(Type requestType)
		where TResponseAssocAttr : WeblinkResponseAttribute
		{
			return GetResponseType<TResponseAssocAttr>(requestType) != null;
		}

		/// <summary>
		/// Invoke any methods found on the target object that have been marked as being response callbacks.
		/// The method can define parameters in any order refering to the handle, request and/or response.
		/// The message handle's request is required to assigned. The response may be null.
		/// </summary>
		/// <param name="target">The object on which to invoke response callback methods</param>
		/// <param name="handle">The handle which contains the request and response values.</param>
		/// <typeparam name="TCallbackAttr">The attribute to look on any of the target's methods.</typeparam>
		/// <typeparam name="TResponseAssocAttr">The attribute that's used to associate a request type with a response type.</typeparam>
		public static void InvokeResponseCallback<TCallbackAttr, TResponseAssocAttr>(object target, IWeblinkMessageHandle handle)
		where TCallbackAttr : WeblinkResponseCallbackAttribute
		where TResponseAssocAttr : WeblinkResponseAttribute
		{
			target.ThrowIfNull(nameof(target));
			handle.ThrowIfNull(nameof(handle));
			handle.ThrowIfNull(nameof(handle.Request));

			Type requestType = handle.Request.GetType();
			if (!IsResponseTypeDefined<TResponseAssocAttr>(requestType))
			{
				return;
			}

			Type handleType = handle.GetType();
			Type targetType = target.GetType();
			Type responseType = GetResponseType<TResponseAssocAttr>(requestType);
			BindingFlags methodFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			while (targetType != ObjectType)
			{
				IEnumerable<MethodInfo> methods = targetType.GetMethods(methodFlags).Where(m => m.IsDefined(typeof(TCallbackAttr), true));
				foreach (MethodInfo callBack in methods)
				{
					TCallbackAttr callbackAttr = callBack.GetCustomAttribute<TCallbackAttr>(false);
					if (!callbackAttr.ResponseType.IsAssignableFrom(responseType))
					{
						continue;
					}

					ParameterInfo[] parametersInfo = callBack.GetParameters();
					object[] parameters = (parametersInfo.Length > 0) ? new object[parametersInfo.Length] : null;
					for (int i = 0; i < parameters.Length; ++i)
					{
						Type parameterType = parametersInfo[i].ParameterType;
						if (parameterType.IsAssignableFrom(handleType))
						{
							parameters[i] = handle;
						}
						else if (parameterType.IsAssignableFrom(requestType))
						{
							parameters[i] = handle.Request;
						}
						else if (parameterType.IsAssignableFrom(responseType))
						{
							parameters[i] = handle.Response;
						}
						else
						{
							parameters[i] = null;
						}
					}

					callBack.Invoke(target, parameters);
				}

				targetType = targetType.BaseType;
			}
		}
	}
}
