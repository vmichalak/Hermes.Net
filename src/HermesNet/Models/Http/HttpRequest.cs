using System;
using System.Collections.Generic;
using System.Net;

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
		public IPAddress Host { get; }

		/// <summary>
		/// Gets the path String.
		/// </summary>
		public string PathString { get; }

		/// <summary>
		/// Gets the url without parameters.
		/// </summary>
		public string BaseUrl { get; }

		/// <summary>
		/// Gets all request parameters.
		/// </summary>
		public IReadOnlyDictionary<string, string> Parameters { get; }

		/// <summary>
		/// Gets the HTTP Method used (post, get, options, ...)
		/// </summary>
		public HttpMethod Method { get; }

		public HttpRequest(string host, string pathString, string baseUrl, Dictionary<string, string> parameters, HttpMethod method)
		{
			if (parameters == null) { throw new ArgumentNullException(nameof(parameters)); }

			IPAddress finalHost;
			if (IPAddress.TryParse(host, out finalHost))
			{
				this.Host = finalHost;
			}
			else
			{
				throw new ArgumentException("Host is not a valid IP adress.");
			}
			this.BaseUrl = baseUrl;
			this.PathString = pathString;
			this.Parameters = parameters;
			this.Method = method;
		}

		/// <summary>
		/// Check if thepParameter exist and if is not empty.
		/// </summary>
		/// <param name="name">Parameter name</param>
		/// <returns>true if exist, false if don't</returns>
		public bool ParameterExist(string name)
		{
			return Parameters.ContainsKey(name) && !string.IsNullOrWhiteSpace(Parameters[name]);
		}
	}
}
