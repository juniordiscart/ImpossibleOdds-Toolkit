namespace ImpossibleOdds.Http
{
	using UnityEngine;

	/// <summary>
	/// Interface to define that the expected response contains a texture.
	/// /// </summary>
	public interface IHttpTextureResponse : IHttpResponse
	{
		Texture2D Texture
		{
			get;
			set;
		}
	}
}
