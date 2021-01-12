namespace ImpossibleOdds.Http
{
	using UnityEngine;

	/// <summary>
	/// Interface to direct the HTTP Messenger to have it download an audio clip asset.
	/// </summary>
	public interface IHttpGetAudioClipRequest : IHttpGetRequest
	{
		AudioType AudioType
		{
			get;
		}
	}
}
