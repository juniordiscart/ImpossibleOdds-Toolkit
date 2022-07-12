namespace ImpossibleOdds.Json
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class JsonRequiredAttribute : Attribute, IRequiredParameter
	{
		private bool performNullCheck = false;

		public bool NullCheck
		{
			get => performNullCheck;
			set => performNullCheck = value;
		}
	}
}
