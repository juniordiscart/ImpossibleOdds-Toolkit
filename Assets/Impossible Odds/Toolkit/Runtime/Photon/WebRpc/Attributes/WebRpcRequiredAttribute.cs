namespace ImpossibleOdds.Photon.WebRpc
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class WebRpcRequiredAttribute : Attribute, IRequiredParameter
	{
		private bool performNullCheck = false;

		public bool NullCheck
		{
			get => performNullCheck;
			set => performNullCheck = value;
		}
	}
}
