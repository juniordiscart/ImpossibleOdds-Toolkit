namespace ImpossibleOdds.Json
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class JsonIndexAttribute : Attribute, IIndexParameter
	{
		public int Index
		{
			get => index;
		}

		private readonly int index;

		public JsonIndexAttribute(int index)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException(string.Format("Index should be greater than 0. {0} given.", index));
			}

			this.index = index;
		}
	}
}
