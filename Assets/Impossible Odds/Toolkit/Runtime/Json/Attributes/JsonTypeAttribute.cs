using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Json
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
	public class JsonTypeAttribute : Attribute, ILookupTypeResolutionParameter
	{
		/// <inheritdoc />
		public Type Target { get; }

		/// <inheritdoc />
		public object Value { get; set; }

		public string KeyOverride { get; set; }

		/// <inheritdoc />
		object ILookupTypeResolutionParameter.KeyOverride => KeyOverride;

		public JsonTypeAttribute(Type target)
		{
			target.ThrowIfNull(nameof(target));
			Target = target;
		}
	}
}