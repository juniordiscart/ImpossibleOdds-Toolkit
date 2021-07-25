namespace ImpossibleOdds.Http
{
	using System;
	using ImpossibleOdds.Weblink;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class HttpResponseTypeAttribute : WeblinkResponseAttribute
	{
		public HttpResponseTypeAttribute(Type responseType)
		: base(responseType)
		{
			if (!typeof(IHttpResponse).IsAssignableFrom(responseType))
			{
				throw new HttpException("Type {0} does not implement interface {1}.", responseType.Name, typeof(IHttpResponse).Name);
			}
		}
	}
}
