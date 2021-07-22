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
		/// <typeparam name="TResponseAttr">Type of attribute to look for on the request type.</typeparam>
		/// <returns>The type of the associated response for the type of request.</returns>
		public static Type GetResponseType<TResponseAttr>(Type requestType)
		where TResponseAttr : WeblinkResponseAttribute
		{
			requestType.ThrowIfNull(nameof(requestType));

			if (!requestResponseMapping.ContainsKey(requestType))
			{
				TResponseAttr attr = requestType.GetCustomAttribute<TResponseAttr>(false);
				requestResponseMapping.Add(requestType, attr?.ResponseType);
			}

			return requestResponseMapping[requestType];
		}

		/// <summary>
		/// Retrieve the associated response type for the given request type.
		/// </summary>
		/// <typeparam name="TRequestType">The type of the request for which to find the type of response.</typeparam>
		/// <typeparam name="TResponseAttr">Type of attribute to look for on the request type.</typeparam>
		/// <returns>The type of the associated response for the type of request.</returns>
		public static Type GetResponseType<TRequestType, TResponseAttr>()
		where TRequestType : IWeblinkRequest
		where TResponseAttr : WeblinkResponseAttribute
		{
			return GetResponseType<TResponseAttr>(typeof(TRequestType));
		}

		/// <summary>
		/// Checks whether a response type is defined for the given request type.
		/// </summary>
		/// <param name="requestType">The type of request for which to check if a type of response is defined.</param>
		/// <typeparam name="TResponseAttr"></typeparam>
		/// <returns>True, if the request type has defined a response type.</returns>
		public static bool IsResponseTypeDefined<TResponseAttr>(Type requestType)
		where TResponseAttr : WeblinkResponseAttribute
		{
			return GetResponseType<TResponseAttr>(requestType) != null;
		}

		/// <summary>
		/// Invoke any methods found on the target object that have been marked as being response callbacks.
		/// The method can define parameters in any order refering to the handle, request and/or response.
		/// </summary>
		/// <param name="target">The object on which to invoke response callback methods</param>
		/// <param name="handle">The handle which contains the request and response values.</param>
		/// <typeparam name="TCallbackAttr">The attribute to look on any of the target's methods.</typeparam>
		public static void InvokeResponseCallback<TCallbackAttr>(object target, IWeblinkMessageHandle handle)
		where TCallbackAttr : WeblinkResponseCallbackAttribute
		{
			target.ThrowIfNull(nameof(target));
			handle.ThrowIfNull(nameof(handle));

			Type targetType = target.GetType();
			BindingFlags methodFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			while (targetType != ObjectType)
			{
				IEnumerable<MethodInfo> methods = targetType.GetMethods(methodFlags).Where(m => m.IsDefined(typeof(TCallbackAttr), true));
				foreach (MethodInfo callBack in methods)
				{
					ParameterInfo[] parametersInfo = callBack.GetParameters();
					object[] parameters = (parametersInfo.Length > 0) ? new object[parametersInfo.Length] : null;
					for (int i = 0; i < parameters.Length; ++i)
					{
						Type parameterType = parametersInfo[i].ParameterType;
						if (HandleType.IsAssignableFrom(parameterType))
						{
							parameters[i] = handle;
						}
						else if (RequestType.IsAssignableFrom(parameterType))
						{
							parameters[i] = handle.Request;
						}
						else if (ResponseType.IsAssignableFrom(parameterType))
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
