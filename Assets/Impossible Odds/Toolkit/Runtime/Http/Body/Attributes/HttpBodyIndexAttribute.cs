namespace ImpossibleOdds.Http
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class HttpBodyIndexAttribute : Attribute, IIndexParameter
	{
		public int Index
		{
			get => index;
		}

		private readonly int index;

		public HttpBodyIndexAttribute(int index)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException(string.Format("Index should be greater than 0. {0} given.", index));
			}

			this.index = index;
		}
	}
}
