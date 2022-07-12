namespace ImpossibleOdds.Weblink
{
	using System;
	using System.Collections.Concurrent;
	using System.Reflection;
	using ImpossibleOdds.ReflectionCaching;
	using ImpossibleOdds.Weblink.Caching;

	public static class WeblinkUtilities
	{
		private readonly static ConcurrentDictionary<Type, Type> requestResponseMapping = new ConcurrentDictionary<Type, Type>();
		private readonly static ConcurrentDictionary<Type, IWeblinkReflectionMap> typeMapCache = new ConcurrentDictionary<Type, IWeblinkReflectionMap>();

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
			return requestResponseMapping.GetOrAdd(requestType, (t) => t.GetCustomAttribute<TResponseAssocAttr>(false)?.ResponseType);
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

			IWeblinkReflectionMap reflectionMap = GetWeblinkReflectionMap(targetType);
			foreach (ITargetedCallback callback in reflectionMap.GetTargetedCallbacks(typeof(TCallbackAttr)))
			{
				if (!callback.ResponseType.IsAssignableFrom(responseType))
				{
					continue;
				}

				if (!callback.Parameters.IsNullOrEmpty())
				{
					object[] parameters = TypeReflectionUtilities.GetParameterInvokationList(callback.Parameters.Length);
					for (int i = 0; i < parameters.Length; ++i)
					{
						Type parameterType = callback.Parameters[i].ParameterType;
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

					callback.Method.Invoke(target, parameters);
					TypeReflectionUtilities.ReturnParameterInvokationList(parameters);
				}
				else
				{
					callback.Method.Invoke(target, null);
				}
			}
		}

		private static IWeblinkReflectionMap GetWeblinkReflectionMap(Type target)
		{
			target.ThrowIfNull(nameof(target));
			return typeMapCache.GetOrAdd(target, (t) => new WeblinkReflectionMap(t));
		}
	}
}
