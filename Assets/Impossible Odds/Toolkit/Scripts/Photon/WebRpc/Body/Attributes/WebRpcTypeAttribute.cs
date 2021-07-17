namespace ImpossibleOdds.Photon.WebRpc
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
	public class WebRpcTypeAttribute : Attribute, ITypeResolveParameter
	{
		private Type target = null;
		private string value = null;

		public Type Target
		{
			get { return target; }
		}

		object ITypeResolveParameter.Value
		{
			get { return Value; }
		}

		public string Value
		{
			get { return string.IsNullOrEmpty(value) ? target.Name : value; }
			set { this.value = value; }
		}

		public WebRpcTypeAttribute(Type target)
		{
			target.ThrowIfNull(nameof(target));
			this.target = target;
		}
	}
}
