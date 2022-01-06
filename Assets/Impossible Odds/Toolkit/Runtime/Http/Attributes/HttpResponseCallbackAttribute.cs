namespace ImpossibleOdds.Http
{
	using System;
	using ImpossibleOdds.Weblink;

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class HttpResponseCallbackAttribute : WeblinkResponseCallbackAttribute
	{
		public HttpResponseCallbackAttribute(Type responseType)
		: base(responseType)
		{
			if (!typeof(IHttpResponse).IsAssignableFrom(responseType))
			{
				throw new HttpException("The type {0} does not implement the {1} interface. This type cannot be used as a response callback type.", responseType.Name, typeof(IHttpResponse).Name);
			}
		}
	}
}
