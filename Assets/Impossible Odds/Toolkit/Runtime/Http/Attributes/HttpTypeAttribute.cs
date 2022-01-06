namespace ImpossibleOdds.Http
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
	public class HttpTypeAttribute : Attribute, ILookupTypeResolveParameter
	{
		private Type target = null;
		private object value = null;
		private string keyOverride = null;

		/// <inheritdoc />
		public Type Target
		{
			get { return target; }
		}

		/// <inheritdoc />
		public object Value
		{
			get { return value; }
			set { this.value = value; }
		}

		/// <inheritdoc />
		object ILookupTypeResolveParameter.KeyOverride
		{
			get { return KeyOverride; }
		}

		/// <inheritdoc />
		public string KeyOverride
		{
			get { return keyOverride; }
			set { keyOverride = value; }
		}

		public HttpTypeAttribute(Type target)
		{
			target.ThrowIfNull(nameof(target));
			this.target = target;
		}
	}
}
