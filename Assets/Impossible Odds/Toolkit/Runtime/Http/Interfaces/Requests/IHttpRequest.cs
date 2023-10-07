using ImpossibleOdds.Weblink;

namespace ImpossibleOdds.Http
{
	public interface IHttpRequest : IWeblinkRequest
	{
		/// <summary>
		/// The URL of the request.
		/// </summary>
		string URL
		{
			get;
		}
	}
}