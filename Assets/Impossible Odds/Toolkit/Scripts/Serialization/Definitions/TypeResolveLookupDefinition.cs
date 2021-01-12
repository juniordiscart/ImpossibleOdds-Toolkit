namespace ImpossibleOdds.Serialization
{
	using System;
	using System.Collections;

	/// <summary>
	/// Abstract generic serialization definiton that only supports lookup-based data structures and has support for type resolvement.
	/// </summary>
	/// <typeparam name="T">Attribute type that defines if an object should be serialized as a lookup-based data structure.</typeparam>
	/// <typeparam name="U">Attribute type for the lookup-based attributes.</typeparam>
	/// <typeparam name="V">Type of the lookup-based container that should get used.</typeparam>
	public abstract class TypeResolveLookupDefinition<T, U, V, W> :
	LookupDefinition<T, U, V>,
	ILookupBasedTypeResolve<W>
	where T : Attribute, ILookupDataStructure
	where U : Attribute, ILookupParameter
	where V : IDictionary
	where W : Attribute, ILookupTypeResolveParameter
	{
		/// <inheritdoc />
		public Type TypeResolveAttribute
		{
			get { return typeof(W); }
		}

		/// <inheritdoc />
		public abstract object TypeResolveKey
		{
			get;
		}
	}
}
