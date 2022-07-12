namespace ImpossibleOdds.Xml
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
	public sealed class XmlObjectAttribute : Attribute, ILookupTypeObject
	{
		private string rootName = string.Empty;

		/// <summary>
		/// Name of the XML root element.
		/// </summary>
		public string RootName
		{
			get => rootName;
			set => rootName = value;
		}
	}
}
