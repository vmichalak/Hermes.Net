using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HermesNet.Models.Http
{
	/// <summary>
	/// Represents the outgoing side of an individual HTTP request. Mutable Object.
	/// </summary>
	public class HttpResponse
	{
		private HttpStatusCode _statusCode = HttpStatusCode.OK;
		private byte[] _body;

		/// <summary>
		/// Http Response Headers. Contains all headers of the response.
		/// </summary>
		public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>(); 

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
		/// Return encoded body.
		/// </summary>
		public byte[] Body => this._body == null ? new byte[] {} : _body;

		/// <summary>
		/// Send the content to the client.
		/// </summary>
		/// <param name="content"></param>
		public void Send(string content)
		{
			this.Send(Encoding.UTF8.GetBytes(content));
		}

		/// <summary>
		/// Send the content to the client.
		/// </summary>
		/// <param name="content"></param>
		public void Send(byte[] content)
		{
			if (!Ended)
			{
				this._body = content;
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
