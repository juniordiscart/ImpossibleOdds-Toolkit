namespace ImpossibleOdds.DataMapping
{
	using System;
	using System.Collections;

	/// <summary>
	/// Abstract generic mapping definiton that implements both index-based and lookup-based mapping definitions and has type resolve support.
	/// </summary>
	/// <typeparam name="T">Attribute type that defines if an object should be mapped as an index-based data structure.</typeparam>
	/// <typeparam name="U">Attribute type for the index-based attributes.</typeparam>
	/// <typeparam name="V">Type of the index-based container that should get used.</typeparam>
	/// <typeparam name="A"></typeparam>
	/// <typeparam name="X">Attribute type that defines if an object should be mapped as a lookup-based data structure.</typeparam>
	/// <typeparam name="Y">Attribute type for the lookup-based attributes.</typeparam>
	/// <typeparam name="Z">Type of the lookup-based container that should get used.</typeparam>
	/// <typeparam name="B"></typeparam>
	public abstract class TypeResolveIndexAndLookupDefinition<T, U, V, A, X, Y, Z, B> : IndexAndLookupDefinition<T, U, V, X, Y, Z>, IIndexTypeResolve<A>, ILookupTypeResolve<B>
	where T : Attribute, IIndexDataStructure
	where U : Attribute, IIndexParameter
	where V : IList
	where A : Attribute, IIndexTypeResolveParameter
	where X : Attribute, ILookupDataStructure
	where Y : Attribute, ILookupParameter
	where Z : IDictionary
	where B : Attribute, ILookupTypeResolveParameter
	{
		Type IIndexTypeResolve.TypeResolveAttribute
		{
			get { return typeof(A); }
		}

		Type ILookupTypeResolve.TypeResolveAttribute
		{
			get { return typeof(B); }
		}
	}
}
