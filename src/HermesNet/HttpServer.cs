using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;
using HermesNet.Helpers;
using HermesNet.Middlewares;
using HermesNet.Models;
using HermesNet.Models.Http;

namespace HermesNet
{
	public class HttpServer : IDisposable
	{
		private const uint BufferSize = 8192;
		private readonly MiddlewareManager _middlewareManager = new MiddlewareManager();
		private readonly StreamSocketListener _listener = new StreamSocketListener();

		//public void AddMiddleware(IMiddleware middleware) { this._middlewareManager.Add(middleware); }

		public void AddAllRoute(string route, IMiddleware middleware) { this._middlewareManager.Add(route, HttpMethod.ALL, middleware); }
		public void AddGetRoute(string route, IMiddleware middleware) { this._middlewareManager.Add(route, HttpMethod.GET, middleware); }
		public void AddPostRoute(string route, IMiddleware middleware) { this._middlewareManager.Add(route, HttpMethod.POST, middleware); }
		public void AddPutRoute(string route, IMiddleware middleware) { this._middlewareManager.Add(route, HttpMethod.PUT, middleware); }
		public void AddDeleteRoute(string route, IMiddleware middleware) { this._middlewareManager.Add(route, HttpMethod.DELETE, middleware); }

		public void AddTransparentFolderRoute(string route, StorageFolder folder) { this._middlewareManager.Add(route, HttpMethod.GET, new FolderMiddleware(folder, route)); }

		public async void Listen(int port)
		{
			if (port < 0 || port > 65535) { throw new ArgumentOutOfRangeException(nameof(port)); }

			this._listener.ConnectionReceived += ProcessRequestAsync;
			await this._listener.BindServiceNameAsync(port.ToString());
			Debug.WriteLine("HttpServer Started.");
		}

		public void Dispose()
		{
			this._listener.Dispose();
			Debug.WriteLine("HttpServer Closed.");
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

			Debug.WriteLine("test 1");
			HttpRequest request = ConvertInputStringToHttpRequest(requestStringBuilder.ToString(), args.Socket.Information.RemoteAddress.CanonicalName);
			HttpResponse response = await this._middlewareManager.Execute(request);
			Debug.WriteLine(response.Body.Length);

			using (IOutputStream output = args.Socket.OutputStream)
			{
				await this.WriteResponseAsync(response, output);
			}
		}

		private HttpRequest ConvertInputStringToHttpRequest(string input, string host)
		{
			string[] inputSplitted = input.Split('\n');
			string[] firstLine = inputSplitted[0].Split(' ');

			HttpMethod method;
			try
			{
				method = (HttpMethod) Enum.Parse(typeof (HttpMethod), firstLine[0]);
			}
			catch (ArgumentException)
			{
				method = HttpMethod.ALL;
			}

			string pathString = firstLine[1];
			string baseUrl = pathString;
			Dictionary<string, List<string>> parameters = new Dictionary<string, List<string>>();
			if (pathString.Contains('?'))
			{
				baseUrl = baseUrl.Substring(0, baseUrl.IndexOf('?'));
				
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
			}

			return new HttpRequest(host, pathString, baseUrl, parameters, method);
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
				await stream.WriteAsync(headerBytes, 0, headerBytes.Length);
				await stream.WriteAsync(response.Body, 0, response.Body.Length);
				await stream.FlushAsync();
			}
		}

		
	}
}
