namespace ImpossibleOdds.Weblink.Caching
{
	using System;
	using System.Reflection;
	using ImpossibleOdds.ReflectionCaching;

	public interface ITargetedCallback : IMemberAttributePair
	{
		/// <summary>
		/// The type of the response the targeted callback is meant for.
		/// </summary>
		Type ResponseType
		{
			get;
		}

		/// <summary>
		/// The method this member pertains to.
		/// </summary>
		MethodInfo Method
		{
			get;
		}

		/// <summary>
		/// The parameters to invoke the callback.
		/// </summary>
		ParameterInfo[] Parameters
		{
			get;
		}
	}
}
