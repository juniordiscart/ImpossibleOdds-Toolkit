using UnityEngine;

namespace ImpossibleOdds.Http
{
	/// <summary>
	/// Interface to define that the expected response is an AssetBundle.
	/// </summary>
	public interface IHttpAssetBundleResponse : IHttpResponse
	{
		/// <summary>
		/// The downloaded asset bundle.
		/// </summary>
		AssetBundle AssetBundle
		{
			get;
			set;
		}
	}
}