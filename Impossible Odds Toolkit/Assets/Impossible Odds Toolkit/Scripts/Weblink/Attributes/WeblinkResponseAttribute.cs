namespace ImpossibleOdds.Weblink
{
	using System;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class WeblinkResponseAttribute : Attribute, IWeblinkResponseTypeAssociation
	{
		public Type ResponseType
		{
			get { return responseType; }
		}

		private readonly Type responseType;

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
