namespace ImpossibleOdds.Http
{
	using System;
	using ImpossibleOdds.Weblink;

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class HttpResponseCallbackAttribute : WeblinkResponseCallbackAttribute
	{
		public HttpResponseCallbackAttribute(Type responseType)
		: base(responseType)
		{ }
	}
}
