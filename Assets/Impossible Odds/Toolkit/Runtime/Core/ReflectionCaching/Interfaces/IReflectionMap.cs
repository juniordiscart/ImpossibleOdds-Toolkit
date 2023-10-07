using System;

namespace ImpossibleOdds.ReflectionCaching
{
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