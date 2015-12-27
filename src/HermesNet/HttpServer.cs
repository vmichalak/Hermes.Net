using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using HermesNet.Helpers;
using HermesNet.Models;
using HermesNet.Models.Http;

namespace HermesNet
{
	public class HttpServer : IDisposable
	{
		private const uint BufferSize = 8192;
		private readonly MiddlewareManager _middlewareManager = new MiddlewareManager();
		private readonly StreamSocketListener _listener = new StreamSocketListener();

		public void AddMiddleware(IMiddleware middleware) { this._middlewareManager.Add(middleware); }

		public async void Run(int port)
		{
			if (port < 0 || port > 65535) { throw new ArgumentOutOfRangeException(nameof(port)); }

			this._listener.ConnectionReceived += ProcessRequestAsync;
			await this._listener.BindServiceNameAsync(port.ToString());
			Debug.WriteLine("HttpServer Started.");
		}

		private async void ProcessRequestAsync(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
		{
			StringBuilder requestStringBuilder = new StringBuilder();
			using (IInputStream input = args.Socket.InputStream)
			{
				byte[] data = new byte[BufferSize];
				IBuffer buffer = data.AsBuffer();
				uint dataRead = BufferSize;
				while (dataRead == BufferSize)
				{
					await input.ReadAsync(buffer, BufferSize, InputStreamOptions.Partial);
					requestStringBuilder.Append(Encoding.UTF8.GetString(data, 0, data.Length));
					dataRead = buffer.Length;
				}
			}

			HttpRequest request = ConvertInputStringToHttpRequest(requestStringBuilder.ToString(), args.Socket.Information.RemoteAddress.CanonicalName);
			HttpResponse response = this._middlewareManager.Execute(request);

			using (IOutputStream output = args.Socket.OutputStream)
			{
				await this.WriteResponseAsync(response, output);
			}
		}

		private HttpRequest ConvertInputStringToHttpRequest(string input, string host)
		{
			string[] inputSplitted = input.Split('\n');
			string[] firstLine = inputSplitted[0].Split(' ');

			HttpMethod method = (HttpMethod)Enum.Parse(typeof(HttpMethod), firstLine[0]);
			string pathString = firstLine[1];
			Dictionary<string, List<string>> parameters = new Dictionary<string, List<string>>();
			try
			{
				string[] parameterStrings = pathString.Substring(pathString.IndexOf('?') + 1).Split('&');
				parameters = parameterStrings
					.Select(
						str => str.Split('=')
					)
					.ToDictionary(
						temp => temp[0],
						temp => new List<string>() {temp[1] ?? ""}
					);
			}
			catch
			{
				// ignored
			}

			return new HttpRequest(host, pathString, parameters, method);
		}

		private async Task WriteResponseAsync(HttpResponse response, IOutputStream output)
		{
			using (Stream stream = output.AsStreamForWrite())
			{
				string header = String.Format("HTTP/1.1 {0} {1}\r\n"
									+ "Content-Length: {2}\r\n"
									+ "Connection: close\r\n\r\n",
									(int)response.StatusCode,
									response.StatusCode.ToString(),
									response.Body.Length
								);
				byte[] headerBytes = Encoding.UTF8.GetBytes(header);
				byte[] bodyBytes = Encoding.UTF8.GetBytes(response.Body);
				await stream.WriteAsync(headerBytes, 0, headerBytes.Length);
				await stream.WriteAsync(bodyBytes, 0, bodyBytes.Length);
				await stream.FlushAsync();
			}
		}

		public void Dispose()
		{
			this._listener.Dispose();
			Debug.WriteLine("HttpServer Closed.");
		}
	}
}
