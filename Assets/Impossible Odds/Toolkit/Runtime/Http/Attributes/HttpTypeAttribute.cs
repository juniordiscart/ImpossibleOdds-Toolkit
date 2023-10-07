using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Http
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
	public class HttpTypeAttribute : Attribute, ILookupTypeResolutionParameter
	{
		/// <inheritdoc />
		public Type Target { get; }

		/// <inheritdoc />
		public object Value { get; set; }

		/// <inheritdoc />
		object ILookupTypeResolutionParameter.KeyOverride => KeyOverride;

		/// <inheritdoc />
		public string KeyOverride { get; set; }

		public HttpTypeAttribute(Type target)
		{
			target.ThrowIfNull(nameof(target));
			Target = target;
		}
	}
}