namespace ImpossibleOdds.Xml
{
	using System;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class XmlObjectAttribute : Attribute
	{
		private string rootName = string.Empty;

		/// <summary>
		/// Name of the XML root element.
		/// </summary>
		public string RootName
		{
			get { return rootName; }
			set { rootName = value; }
		}
	}
}
