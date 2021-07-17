namespace ImpossibleOdds.Photon.WebRpc
{
	public sealed class WebRpcException : ImpossibleOddsException
	{
		public WebRpcException()
		{ }

		public WebRpcException(string errMsg)
		: base(errMsg)
		{ }

		public WebRpcException(string errMsg, params object[] format)
		: base(string.Format(errMsg, format))
		{ }
	}
}
