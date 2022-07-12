namespace ImpossibleOdds.ReflectionCaching
{
	using System;

	public interface IReflectionMap
	{
		/// <summary>
		/// The type the reflection map applies to.
		/// </summary>
		Type Type
		{
			get;
		}
	}
}
