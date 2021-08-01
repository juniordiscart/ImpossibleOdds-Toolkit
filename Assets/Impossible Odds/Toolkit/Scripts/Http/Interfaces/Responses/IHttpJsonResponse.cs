﻿namespace ImpossibleOdds.Http
{
	using System;

	/// <summary>
	/// Denotes the response is expected to be JSON data, and will have the returned data to be applied as such.
	/// </summary>
	[Obsolete("Use the IHttpPostResponse interface instead.")]
	public interface IHttpJsonResponse : IHttpPostResponse
	{ }
}
