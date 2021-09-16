namespace ImpossibleOdds.Xml
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
	public class XmlTypeAttribute : Attribute, ITypeResolveParameter
	{
		private Type target = null;
		private string value = null;

		/// <inheritdoc />
		public Type Target
		{
			get { return target; }
		}

		/// <inheritdoc />
		object ITypeResolveParameter.Value
		{
			get { return Value; }
		}

		/// <inheritdoc />
		public string Value
		{
			get { return string.IsNullOrEmpty(value) ? target.Name : value; }
			set { this.value = value; }
		}

		public XmlTypeAttribute(Type target)
		{
			target.ThrowIfNull(nameof(target));
			this.target = target;
		}
	}
}
