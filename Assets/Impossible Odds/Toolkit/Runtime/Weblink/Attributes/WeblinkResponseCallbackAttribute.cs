using System;

namespace ImpossibleOdds.Weblink
{
	[AttributeUsage(AttributeTargets.Method)]
	public abstract class WeblinkResponseCallbackAttribute : Attribute, IWeblinkResponseTypeAssociation
	{
		public Type ResponseType { get; }

		public WeblinkResponseCallbackAttribute(Type responseType)
		{
			responseType.ThrowIfNull(nameof(responseType));

			if (!typeof(IWeblinkResponse).IsAssignableFrom(responseType))
			{
				throw new WeblinkException("Type {0} does not implement interface {1}.", responseType.Name, nameof(IWeblinkResponse));
			}

			ResponseType = responseType;
		}
	}
}