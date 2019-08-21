namespace ImpossibleOdds.Json
{
	using System;
	using ImpossibleOdds.DataMapping;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class JsonIndexAttribute : Attribute, IIndexParameter
	{
		public int Index
		{
			get { return index; }
		}

		private readonly int index;

		public JsonIndexAttribute(int index)
		{
			if (index < 0)
			{
				throw new ArgumentException(string.Format("Index should be greater than 0. {0} given.", index));
			}

			this.index = index;
		}
	}
}
