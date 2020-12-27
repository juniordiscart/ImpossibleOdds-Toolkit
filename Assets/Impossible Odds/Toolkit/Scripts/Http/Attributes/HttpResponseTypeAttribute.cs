namespace ImpossibleOdds.Http
{
	using System;
	using ImpossibleOdds.Weblink;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class HttpResponseTypeAttribute : WeblinkResponseAttribute
	{
		public HttpResponseTypeAttribute(Type responseType)
		: base(responseType)
		{ }
	}
}
