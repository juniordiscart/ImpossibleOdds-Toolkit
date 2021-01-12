namespace ImpossibleOdds.Http
{
	using UnityEngine;

	/// <summary>
	/// Interface to define that the expected response is an AudioClip.
	/// </summary>
	public interface IHttpAudioClipResponse : IHttpResponse
	{
		AudioClip AudioClip
		{
			get;
			set;
		}
	}
}
