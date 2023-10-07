using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Xml
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
	public class XmlTypeAttribute : Attribute, ITypeResolutionParameter
	{
		/// <inheritdoc />
		public Type Target { get; }

		/// <inheritdoc />
		public object Value { get; set; }

		/// <inheritdoc />
		public string KeyOverride { get; set; }

		/// <summary>
		/// By default, type information is saved in an XML Attribute.
		/// Setting this to true will save the type information in an XML Element instead.
		/// </summary>
		public bool SetAsElement { get; set; }

		public XmlTypeAttribute(Type target)
		{
			target.ThrowIfNull(nameof(target));
			Target = target;
		}
	}
}