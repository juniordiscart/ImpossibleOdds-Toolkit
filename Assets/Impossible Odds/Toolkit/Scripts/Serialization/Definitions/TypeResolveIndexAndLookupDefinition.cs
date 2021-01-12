namespace ImpossibleOdds.Serialization
{
	using System;
	using System.Collections;

	/// <summary>
	/// Abstract generic serialization definiton that supports both index-based and lookup-based serialized data and has support for resolving types.
	/// </summary>
	/// <typeparam name="T">Attribute type that defines if an object should be (de)serialized as an index-based data structure.</typeparam>
	/// <typeparam name="U">Attribute type for the index-based attributes.</typeparam>
	/// <typeparam name="V">Type of the index-based container that should get used.</typeparam>
	/// <typeparam name="A">Type of the index-based attribute for type resolvement.</typeparam>
	/// <typeparam name="X">Attribute type that defines if an object should be (de)serialized as a lookup-based data structure.</typeparam>
	/// <typeparam name="Y">Attribute type for the lookup-based attributes.</typeparam>
	/// <typeparam name="Z">Type of the lookup-based container that should get used.</typeparam>
	/// <typeparam name="B">Type of the lookup-based attribute for type resolvement.</typeparam>
	public abstract class TypeResolveIndexAndLookupDefinition<T, U, V, A, X, Y, Z, B> :
	IndexAndLookupDefinition<T, U, V, X, Y, Z>,
	IIndexBasedTypeResolve<A>,
	ILookupBasedTypeResolve<B>
	where T : Attribute, IIndexDataStructure
	where U : Attribute, IIndexParameter
	where V : IList
	where A : Attribute, IIndexTypeResolveParameter
	where X : Attribute, ILookupDataStructure
	where Y : Attribute, ILookupParameter
	where Z : IDictionary
	where B : Attribute, ILookupTypeResolveParameter
	{
		/// <inheritdoc />
		public abstract object TypeResolveKey
		{
			get;
		}

		/// <inheritdoc />
		public int TypeResolveIndex
		{
			get;
		}

		/// <inheritdoc />
		Type IIndexBasedTypeResolve.TypeResolveAttribute
		{
			get { return typeof(A); }
		}

		/// <inheritdoc />
		Type ILookupBasedTypeResolve.TypeResolveAttribute
		{
			get { return typeof(B); }
		}
	}
}
