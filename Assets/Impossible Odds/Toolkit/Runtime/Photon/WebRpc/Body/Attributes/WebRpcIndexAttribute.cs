namespace ImpossibleOdds.Photon.WebRpc
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class WebRpcIndexAttribute : Attribute, IIndexParameter
	{
		public int Index
		{
			get => index;
		}

		private readonly int index;

		public WebRpcIndexAttribute(int index)
		{
			if (index < 0)
			{
				throw new ArgumentException(string.Format("Index should be greater than 0. {0} given.", index));
			}

			this.index = index;
		}
	}
}
