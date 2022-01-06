namespace ImpossibleOdds.Http
{
	using ImpossibleOdds.Weblink;

	public interface IHttpRequest : IWeblinkRequest
	{
		string URL
		{
			get;
		}
	}
}
