using System;

namespace ImpossibleOdds.Weblink
{
	[AttributeUsage(AttributeTargets.Class)]
	public abstract class WeblinkResponseAttribute : Attribute, IWeblinkResponseTypeAssociation
	{
		public Type ResponseType { get; }

		public WeblinkResponseAttribute(Type responseType)
		{
			responseType.ThrowIfNull(nameof(responseType));

			if (!typeof(IWeblinkResponse).IsAssignableFrom(responseType))
			{
				throw new WeblinkException("Type {0} does not implement interface {1}.", responseType.Name, nameof(IWeblinkResponse));
			}

			if (responseType.IsInterface || responseType.IsAbstract)
			{
				throw new WeblinkException("Type {0} is not allowed to be abstract or an interface.", responseType.Name);
			}

			ResponseType = responseType;
		}
	}
}