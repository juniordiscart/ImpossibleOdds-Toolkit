namespace ImpossibleOdds.Serialization
{
	using System;

	/// <summary>
	/// Callback attribute register.
	/// </summary>
	public interface ICallbacksSupport
	{
		/// <summary>
		/// Attribute type that denotes serialization of the object is about to start.
		/// </summary>
		Type OnSerializationCallbackType
		{
			get;
		}

		/// <summary>
		/// Attribute type that denotes serialization of the object has finished.
		/// </summary>
		Type OnSerializedCallbackType
		{
			get;
		}

		/// <summary>
		/// Attribute type that denotes deserialization of the object about to start.
		/// </summary>
		Type OnDeserializionCallbackType
		{
			get;
		}

		/// <summary>
		/// Attribute type that denotes deserialization of the object has finished.
		/// </summary>
		Type OnDeserializedCallbackType
		{
			get;
		}
	}

	/// <summary>
	/// Generic callback attribute register.
	/// </summary>
	/// <typeparam name="T">Attribute type that denotes serialization of the object is about to start.</typeparam>
	/// <typeparam name="U">Attribute type that denotes serialization of the object has finished.</typeparam>
	/// <typeparam name="V">Attribute type that denotes deserialization of the object about to start.</typeparam>
	/// <typeparam name="W">Attribute type that denotes deserialization of the object has finished.</typeparam>
	public interface ICallbacksSupport<T, U, V, W> : ICallbacksSupport
	where T : Attribute
	where U : Attribute
	where V : Attribute
	where W : Attribute
	{ }
}
