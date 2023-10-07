using UnityEngine;

namespace ImpossibleOdds.Http
{
	/// <summary>
	/// Interface to direct the HTTP Messenger to have it download an audio clip asset.
	/// </summary>
	public interface IHttpGetAudioClipRequest : IHttpGetRequest
	{
		/// <summary>
		/// Type of audio clip.
		/// </summary>
		AudioType AudioType
		{
			get;
		}
	}
}