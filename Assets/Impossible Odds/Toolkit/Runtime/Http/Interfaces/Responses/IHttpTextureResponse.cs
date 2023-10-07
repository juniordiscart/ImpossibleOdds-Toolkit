using UnityEngine;

namespace ImpossibleOdds.Http
{
	/// <summary>
	/// Interface to define that the expected response contains a texture.
	/// /// </summary>
	public interface IHttpTextureResponse : IHttpResponse
	{
		/// <summary>
		/// The downloaded texture.
		/// </summary>
		Texture2D Texture
		{
			get;
			set;
		}
	}
}