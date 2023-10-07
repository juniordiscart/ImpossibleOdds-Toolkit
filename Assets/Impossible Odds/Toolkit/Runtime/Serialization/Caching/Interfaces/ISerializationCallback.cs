using System.Reflection;

namespace ImpossibleOdds.Serialization.Caching
{
	/// <summary>
	/// Interface for caching common info of a callback method of an object.
	/// </summary>
	public interface ISerializationCallback : ISerializableMethod
	{
		/// <summary>
		/// The parameters to invoke the callback.
		/// </summary>
		ParameterInfo[] Parameters
		{
			get;
		}
	}
}