using System;
using ImpossibleOdds.DataMapping;

namespace ImpossibleOdds.Http
{
	public sealed class HttpURLFieldAttribute : Attribute, ILookupParameter
	{
		public object Key
		{
			get { return key; }
		}

		private readonly object key;

		public HttpURLFieldAttribute(object key)
		{
			key.ThrowIfNull(nameof(key));
			this.key = key;
		}
	}
}
