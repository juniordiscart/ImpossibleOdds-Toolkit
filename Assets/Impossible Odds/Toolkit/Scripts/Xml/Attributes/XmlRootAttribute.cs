namespace ImpossibleOdds.Xml
{
	using System;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class XmlRootAttribute : Attribute
	{
		private readonly string rootName = string.Empty;

		public string RootName
		{
			get { return rootName; }
		}

		public XmlRootAttribute(string rootName)
		{
			rootName.ThrowIfNullOrWhitespace(nameof(rootName));
			this.rootName = rootName;
		}
	}
}
