namespace ImpossibleOdds.Xml
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
	public class XmlTypeAttribute : Attribute, ILookupTypeResolveParameter
	{
		private Type target = null;
		private object value = null;
		private string keyOverride = null;
		private bool setAsElement = false;

		/// <inheritdoc />
		public Type Target
		{
			get => target;
		}

		/// <inheritdoc />
		public object Value
		{
			get => value;
			set => this.value = value;
		}

		/// <inheritdoc />
		object ILookupTypeResolveParameter.KeyOverride
		{
			get => keyOverride;
		}

		/// <inheritdoc />
		public string KeyOverride
		{
			get => keyOverride;
			set => keyOverride = value;
		}

		/// <summary>
		/// By default, type information is saved in an XML Attribute.
		/// Setting this to true will save the type information in an XML Element instead.
		/// </summary>
		public bool SetAsElement
		{
			get => setAsElement;
			set => setAsElement = value;
		}

		public XmlTypeAttribute(Type target)
		{
			target.ThrowIfNull(nameof(target));
			this.target = target;
		}
	}
}
