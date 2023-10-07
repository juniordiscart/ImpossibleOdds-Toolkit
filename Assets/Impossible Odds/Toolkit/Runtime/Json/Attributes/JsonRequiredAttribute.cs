using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Json
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class JsonRequiredAttribute : Attribute, IRequiredParameter
	{
		public bool NullCheck { get; set; }
	}
}