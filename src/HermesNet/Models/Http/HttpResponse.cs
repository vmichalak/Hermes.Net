using System;
using System.Net;

namespace HermesNet.Models.Http
{
	/// <summary>
	/// Represents the outgoing side of an individual HTTP request. Mutable Object.
	/// </summary>
	public class HttpResponse
	{
		private HttpStatusCode _statusCode = HttpStatusCode.OK;

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
		public string Body { get; private set; } = "";

		/// <summary>
		/// Send the content to the client.
		/// </summary>
		/// <param name="content"></param>
		public void Send(string content)
		{
			if (!Ended)
			{
				this.Body = content;
				this.End();
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
	}
}
