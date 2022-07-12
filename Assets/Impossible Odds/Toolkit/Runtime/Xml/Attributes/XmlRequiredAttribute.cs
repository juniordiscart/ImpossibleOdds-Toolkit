namespace ImpossibleOdds.Xml
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class XmlRequiredAttribute : Attribute, IRequiredParameter
	{
		private bool performNullCheck = false;

		public bool NullCheck
		{
			get => performNullCheck;
			set => performNullCheck = value;
		}
	}
}
