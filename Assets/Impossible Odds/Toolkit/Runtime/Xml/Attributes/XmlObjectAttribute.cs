using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Xml
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public sealed class XmlObjectAttribute : Attribute, ILookupTypeObject
	{
		/// <summary>
		/// Name of the XML root element.
		/// </summary>
		public string RootName { get; set; } = string.Empty;
	}
}