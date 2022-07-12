namespace ImpossibleOdds.Weblink
{
	using System;

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public abstract class WeblinkResponseCallbackAttribute : Attribute, IWeblinkResponseTypeAssociation
	{
		private readonly Type responseType;

		public Type ResponseType
		{
			get => responseType;
		}

		public WeblinkResponseCallbackAttribute(Type responseType)
		{
			responseType.ThrowIfNull(nameof(responseType));

			if (!typeof(IWeblinkResponse).IsAssignableFrom(responseType))
			{
				throw new WeblinkException("Type {0} does not implement interface {1}.", responseType.Name, typeof(IWeblinkResponse).Name);
			}

			this.responseType = responseType;
		}
	}
}
