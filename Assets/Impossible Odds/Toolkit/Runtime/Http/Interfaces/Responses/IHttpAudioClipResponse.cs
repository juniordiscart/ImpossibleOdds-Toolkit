using UnityEngine;

namespace ImpossibleOdds.Http
{
	/// <summary>
	/// Interface to define that the expected response is an AudioClip.
	/// </summary>
	public interface IHttpAudioClipResponse : IHttpResponse
	{
		/// <summary>
		/// The downloaded audio clip.
		/// </summary>
		AudioClip AudioClip
		{
			get;
			set;
		}
	}
}