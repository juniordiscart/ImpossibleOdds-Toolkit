using System;
using ImpossibleOdds.DataMapping;

namespace ImpossibleOdds.Http
{
	public sealed class HttpBodyFieldAttribute : Attribute, ILookupParameter
	{
		public object Key
		{
			get { return key; }
		}

		private readonly object key;

		public HttpBodyFieldAttribute(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}

			this.key = key;
		}
	}
}
