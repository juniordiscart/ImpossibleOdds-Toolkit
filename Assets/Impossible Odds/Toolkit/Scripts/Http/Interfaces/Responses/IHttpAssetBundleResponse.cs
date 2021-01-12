namespace ImpossibleOdds.Http
{
	using UnityEngine;

	/// <summary>
	/// Interface to define that the expected response is an AssetBundle.
	/// </summary>
	public interface IHttpAssetBundleResponse : IHttpResponse
	{
		AssetBundle AssetBundle
		{
			get;
			set;
		}
	}
}
