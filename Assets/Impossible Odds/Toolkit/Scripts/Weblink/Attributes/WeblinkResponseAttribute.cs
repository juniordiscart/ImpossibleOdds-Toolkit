namespace ImpossibleOdds.Weblink
{
	using System;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public abstract class WeblinkResponseAttribute : Attribute, IWeblinkResponseTypeAssociation
	{
		private readonly Type responseType;

		public Type ResponseType
		{
			get { return responseType; }
		}

		public WeblinkResponseAttribute(Type responseType)
		{
			if (!typeof(IWeblinkResponse).IsAssignableFrom(responseType))
			{
				throw new WeblinkException(string.Format("Type {0} does not implement interface {1}.", responseType.Name, typeof(IWeblinkResponse).Name));
			}

			this.responseType = responseType;
		}
	}
}
