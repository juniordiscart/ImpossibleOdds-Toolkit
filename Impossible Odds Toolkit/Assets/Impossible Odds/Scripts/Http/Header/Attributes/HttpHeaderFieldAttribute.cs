using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Http
{
	public sealed class HttpHeaderFieldAttribute : Attribute, ILookupParameter
	{
		public object Key
		{
			get { return key; }
		}

		private readonly object key;

		public HttpHeaderFieldAttribute(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}

			this.key = key;
		}
	}
}
