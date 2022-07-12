namespace ImpossibleOdds.Weblink
{
	using System;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public abstract class WeblinkResponseAttribute : Attribute, IWeblinkResponseTypeAssociation
	{
		private readonly Type responseType;

		public Type ResponseType
		{
			get => responseType;
		}

		public WeblinkResponseAttribute(Type responseType)
		{
			responseType.ThrowIfNull(nameof(responseType));

			if (!typeof(IWeblinkResponse).IsAssignableFrom(responseType))
			{
				throw new WeblinkException("Type {0} does not implement interface {1}.", responseType.Name, typeof(IWeblinkResponse).Name);
			}
			else if (responseType.IsInterface || responseType.IsAbstract)
			{
				throw new WeblinkException("Type {0} is not allowed to be abstract or an interface.", responseType.Name);
			}

			this.responseType = responseType;
		}
	}
}
