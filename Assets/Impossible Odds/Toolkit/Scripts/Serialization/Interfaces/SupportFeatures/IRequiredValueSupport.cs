namespace ImpossibleOdds.Serialization
{
	using System;

	/// <summary>
	/// Implement to denote support for required values during deserialization.
	/// </summary>
	public interface IRequiredValueSupport
	{
		/// <summary>
		/// Attribute type to define a value being required in the data.
		/// </summary>
		Type RequiredAttributeType
		{
			get;
		}
	}

	/// <summary>
	/// Generic variant with restrictions on the generic type parameter.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IRequiredValueSupport<T> : IRequiredValueSupport
	where T : Attribute
	{ }
}
