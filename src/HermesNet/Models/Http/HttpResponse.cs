using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace HermesNet.Models.Http
{
	/// <summary>
	/// Represents the outgoing side of an individual HTTP request. Mutable Object.
	/// </summary>
	public class HttpResponse
	{
		private readonly Dictionary<string, object> _bodyObjects = new Dictionary<string, object>();
		private HttpStatusCode _statusCode = HttpStatusCode.NotFound;

		/// <summary>
		/// Return the response end status.
		/// If the end status is true, you cannot edit the response.
		/// </summary>
		public bool Ended { get; private set; } = false;

		/// <summary>
		/// Http Response Status Code
		/// </summary>
		public HttpStatusCode StatusCode
		{
			get { return this._statusCode; }
			set
			{
				if (!Ended)
				{
					this._statusCode = value;
				}
				else
				{
					throw new UnauthorizedAccessException("HttpResponse is ended, edits are forbidden.");
				}
			}
		}

		/// <summary>
		/// Return Json string of the body.
		/// </summary>
		public string Body => JsonConvert.SerializeObject(_bodyObjects);

		/// <summary>
		/// Add Content to the body
		/// </summary>
		/// <param name="key">Name of the content</param>
		/// <param name="value">Content</param>
		public void AddToBody(string key, object value)
		{
			if (!Ended)
			{
				this._bodyObjects.Add(key, value);
				StatusCode = HttpStatusCode.OK;
			}
			else
			{
				throw new UnauthorizedAccessException("HttpResponse is ended, edits are forbidden.");
			}
		}

		/// <summary>
		/// Close all modifications on Response.
		/// </summary>
		public void End()
		{
			this.Ended = true;
		}

		/// <summary>
		/// Close all modifications on Response for catching an Error.
		/// </summary>
		/// <param name="statusCode">HTTP Status Code of the error</param>
		/// <param name="errorMessage">Error Message</param>
		public void ErrorEnd(HttpStatusCode statusCode, string errorMessage)
		{
			this._bodyObjects.Clear();
			this.AddToBody("message", errorMessage);
			this.StatusCode = statusCode;
			this.End();
		}
	}
}
