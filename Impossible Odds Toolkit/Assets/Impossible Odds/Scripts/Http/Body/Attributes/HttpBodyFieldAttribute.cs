namespace ImpossibleOdds.Http
{
	using System;
	using ImpossibleOdds.DataMapping;

	public sealed class HttpBodyFieldAttribute : Attribute, ILookupParameter
	{
		public object Key
		{
			get { return key; }
		}

		private readonly object key;

		public HttpBodyFieldAttribute(object key)
		{
			key.ThrowIfNull(nameof(key));
			this.key = key;
		}
	}
}
