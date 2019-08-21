namespace ImpossibleOdds.DataMapping
{
	using System;
	using System.Collections;

	/// <summary>
	/// Abstract generic mapping definiton that implements only a lookup-based mapping definition.
	/// </summary>
	/// <typeparam name="T">Attribute type that defines if an object should be mapped as a lookup-based data structure.</typeparam>
	/// <typeparam name="U">Attribute type for the lookup-based attributes.</typeparam>
	/// <typeparam name="V">Type of the lookup-based container that should get used.</typeparam>
	public abstract class TypeResolveLookupDefinition<T, U, V, W> : LookupDefinition<T, U, V>, ILookupTypeResolve<W>
	where T : Attribute, ILookupDataStructure
	where U : Attribute, ILookupParameter
	where V : IDictionary
	where W : Attribute, ILookupTypeResolveParameter
	{
		public Type TypeResolveAttribute
		{
			get { return typeof(W); }
		}
	}
}
