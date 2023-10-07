using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Http
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class HttpBodyRequiredAttribute : Attribute, IRequiredParameter
	{
		public bool NullCheck { get; set; }
	}
}