namespace ImpossibleOdds.Http
{
	using System;
	using ImpossibleOdds.Weblink;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class HttpResponseAssociationAttribute : WeblinkResponseAttribute
	{
		public HttpResponseAssociationAttribute(Type responseType)
		: base(responseType)
		{ }
	}
}
