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

		/// <summary>
		/// Use a middleware on all requests.
		/// </summary>
		/// <param name="middleware"></param>
		public void Use(IMiddleware middleware) { this._middlewareManager.Add(middleware); }

		/// <summary>
		/// Use a middleware on all requests.
		/// </summary>
		/// <param name="middleware"></param>
		public void Use(Action<HttpContext> middleware) { this._middlewareManager.Add(middleware); }

		/// <summary>
		/// Routes HTTP requests to the specified path with the specified middleware.
		/// </summary>
		/// <param name="route"></param>
		/// <param name="middleware"></param>
		public void AddAllRoute(string route, IMiddleware middleware) { this._middlewareManager.AddToRoute(route, HttpMethod.ALL, middleware); }

		/// <summary>
		/// Routes HTTP requests to the specified path with the specified middleware.
		/// </summary>
		/// <param name="route"></param>
		/// <param name="middleware"></param>
		public void AddAllRoute(string route, Action<HttpContext> middleware) { this._middlewareManager.AddToRoute(route, HttpMethod.ALL, middleware); }

		/// <summary>
		/// Routes HTTP GET requests to the specified path with the specified middleware.
		/// </summary>
		/// <param name="route"></param>
		/// <param name="middleware"></param>
		public void AddGetRoute(string route, IMiddleware middleware) { this._middlewareManager.AddToRoute(route, HttpMethod.GET, middleware); }

		/// <summary>
		/// Routes HTTP GET requests to the specified path with the specified middleware.
		/// </summary>
		/// <param name="route"></param>
		/// <param name="middleware"></param>
		public void AddGetRoute(string route, Action<HttpContext> middleware) { this._middlewareManager.AddToRoute(route, HttpMethod.GET, middleware); }

		/// <summary>
		/// Routes HTTP POST requests to the specified path with the specified middleware.
		/// </summary>
		/// <param name="route"></param>
		/// <param name="middleware"></param>
		public void AddPostRoute(string route, IMiddleware middleware) { this._middlewareManager.AddToRoute(route, HttpMethod.POST, middleware); }

		/// <summary>
		/// Routes HTTP POST requests to the specified path with the specified middleware.
		/// </summary>
		/// <param name="route"></param>
		/// <param name="middleware"></param>
		public void AddPostRoute(string route, Action<HttpContext> middleware) { this._middlewareManager.AddToRoute(route, HttpMethod.POST, middleware); }

		/// <summary>
		/// Routes HTTP PUT requests to the specified path with the specified middleware.
		/// </summary>
		/// <param name="route"></param>
		/// <param name="middleware"></param>
		public void AddPutRoute(string route, IMiddleware middleware) { this._middlewareManager.AddToRoute(route, HttpMethod.PUT, middleware); }

		/// <summary>
		/// Routes HTTP PUT requests to the specified path with the specified middleware.
		/// </summary>
		/// <param name="route"></param>
		/// <param name="middleware"></param>
		public void AddPutRoute(string route, Action<HttpContext> middleware) { this._middlewareManager.AddToRoute(route, HttpMethod.PUT, middleware); }

		/// <summary>
		/// Routes HTTP DELETE requests to the specified path with the specified middleware.
		/// </summary>
		/// <param name="route"></param>
		/// <param name="middleware"></param>
		public void AddDeleteRoute(string route, IMiddleware middleware) { this._middlewareManager.AddToRoute(route, HttpMethod.DELETE, middleware); }

		/// <summary>
		/// Routes HTTP DELETE requests to the specified path with the specified middleware.
		/// </summary>
		/// <param name="route"></param>
		/// <param name="middleware"></param>
		public void AddDeleteRoute(string route, Action<HttpContext> middleware) { this._middlewareManager.AddToRoute(route, HttpMethod.DELETE, middleware); }

		/// <summary>
		/// Routes HTTP GET requests to the specified path with the specified folder content.
		/// </summary>
		/// <param name="route"></param>
		/// <param name="folder"></param>
		public void AddTransparentFolderRoute(string route, StorageFolder folder) { this._middlewareManager.AddToRoute(route, HttpMethod.GET, new FolderMiddleware(folder, route)); }

		/// <summary>
		/// Binds and listens for connections on the specified host and port.
		/// </summary>
		/// <param name="port">Port number</param>
		public async void Listen(int port)
		{
			if (port < 0 || port > 65535) { throw new ArgumentOutOfRangeException(nameof(port)); }

			this._listener.ConnectionReceived += ProcessRequestAsync;
			await this._listener.BindServiceNameAsync(port.ToString());
			Debug.WriteLine("HttpServer Started.");
		}

		/// <summary>
		/// Close and release the HttpServer.
		/// </summary>
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
			
			HttpRequest request = ConvertInputStringToHttpRequest(requestStringBuilder.ToString(), args.Socket.Information.RemoteAddress.CanonicalName);
			HttpResponse response = await this._middlewareManager.Execute(request);

			using (IOutputStream output = args.Socket.OutputStream)
			{
				await this.WriteResponseAsync(response, output);
			}
		}

		private HttpRequest ConvertInputStringToHttpRequest(string input, string host)
		{
			string[] firstLine = input.Split('\n')[0].Split(' ');

			HttpMethod method;
			if (!Enum.TryParse(firstLine[0], out method))
			{
				method = HttpMethod.ALL;
			}

			string pathString = firstLine[1];
			string baseUrl = pathString;
			if (baseUrl.Contains('?'))
			{
				baseUrl = baseUrl.Substring(0, baseUrl.IndexOf('?'));
			}
			Dictionary<string, string> parameters = Converters.ConvertNameValueCollectionToDictionary(HttpUtility.ParseQueryString(pathString));
			string body = input.Split(new string[] {"\r\n\r\n"}, StringSplitOptions.None)[1];

			return new HttpRequest(host, pathString, baseUrl, parameters, method, body);
		}

		private async Task WriteResponseAsync(HttpResponse response, IOutputStream output)
		{
			using (Stream stream = output.AsStreamForWrite())
			{
				StringBuilder headerBuilder = new StringBuilder("HTTP/1.1 ").Append((int) response.StatusCode).Append(" ").Append(response.StatusCode).Append("\r\n");
				headerBuilder.Append("Content-Length: ").Append(response.Body.Length).Append("\r\n");
				foreach (KeyValuePair<string, string> header in response.Headers)
				{
					headerBuilder.Append(header.Key).Append(": ").Append(header.Value).Append("\r\n");
				}
				headerBuilder.Append("Connection: close\r\n\r\n");

				byte[] headerBytes = Encoding.UTF8.GetBytes(headerBuilder.ToString());
				await stream.WriteAsync(headerBytes, 0, headerBytes.Length);
				await stream.WriteAsync(response.Body, 0, response.Body.Length);
				await stream.FlushAsync();
			}
		}

		
	}
}
