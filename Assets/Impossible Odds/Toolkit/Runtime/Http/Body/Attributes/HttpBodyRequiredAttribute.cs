using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Http
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class HttpBodyRequiredAttribute : Attribute, IRequiredParameter
	{
		private bool performNullCheck = false;

		public bool NullCheck
		{
			get => performNullCheck;
			set => performNullCheck = value;
		}
	}
}