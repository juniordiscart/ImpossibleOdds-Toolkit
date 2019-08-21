using System;
using ImpossibleOdds.DataMapping;

namespace ImpossibleOdds.Http
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class HttpBodyIndexAttribute : Attribute, IIndexParameter
	{
		public int Index
		{
			get { return index; }
		}

		private readonly int index;

		public HttpBodyIndexAttribute(int index)
		{
			if (index < 0)
			{
				throw new ArgumentException(string.Format("Index should be greater than 0. {0} given.", index));
			}

			this.index = index;
		}
	}
}
