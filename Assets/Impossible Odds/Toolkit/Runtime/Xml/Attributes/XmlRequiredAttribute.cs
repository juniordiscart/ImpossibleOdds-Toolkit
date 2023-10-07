using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Xml
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class XmlRequiredAttribute : Attribute, IRequiredParameter
	{
		public bool NullCheck { get; set; }
	}
}