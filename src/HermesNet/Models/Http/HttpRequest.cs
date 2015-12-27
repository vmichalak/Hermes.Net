using System;
using System.Collections.Generic;

namespace HermesNet.Models.Http
{
	/// <summary>
	/// Represents the incoming side of an individual HTTP request.
	/// </summary>
	public class HttpRequest
	{
		/// <summary>
		/// Gets the client remote IP address.
		/// </summary>
		public string Host { get; }

		/// <summary>
		/// Gets the path String.
		/// </summary>
		public string PathString { get; }

		/// <summary>
		/// Gets all request parameters.
		/// </summary>
		public IReadOnlyDictionary<string, List<string>> Parameters { get; }

		/// <summary>
		/// Gets the HTTP Method used (post, get, options, ...)
		/// </summary>
		public HttpMethod Method { get; }

		public HttpRequest(string host, string pathString, Dictionary<string, List<string>> parameters, HttpMethod method)
		{
			if (parameters == null) { throw new ArgumentNullException(nameof(parameters)); }

			this.Host = host;
			this.PathString = pathString;
			this.Parameters = parameters;
			this.Method = method;
		}
	}
}
