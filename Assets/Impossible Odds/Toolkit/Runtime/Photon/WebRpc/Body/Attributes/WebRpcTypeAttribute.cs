using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Photon.WebRpc
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
	public class WebRpcTypeAttribute : Attribute, ILookupTypeResolutionParameter
	{
		/// <inheritdoc />
		public Type Target { get; }

		/// <inheritdoc />
		public object Value { get; set; }

		/// <inheritdoc />
		object ILookupTypeResolutionParameter.KeyOverride => KeyOverride;

		/// <inheritdoc />
		public string KeyOverride { get; set; }

		public WebRpcTypeAttribute(Type target)
		{
			target.ThrowIfNull(nameof(target));
			Target = target;
		}
	}
}