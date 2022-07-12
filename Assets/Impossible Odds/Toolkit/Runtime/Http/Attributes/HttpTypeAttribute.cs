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
			get => KeyOverride;
		}

		/// <inheritdoc />
		public string KeyOverride
		{
			get => keyOverride;
			set => keyOverride = value;
		}

		public HttpTypeAttribute(Type target)
		{
			target.ThrowIfNull(nameof(target));
			this.target = target;
		}
	}
}
