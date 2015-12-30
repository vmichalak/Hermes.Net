using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.Storage.Streams;
using HermesNet.Models;
using HermesNet.Models.Http;

namespace HermesNet.Middlewares
{
	internal class FolderMiddleware : IMiddleware
	{
		private readonly StorageFolder _folder;
		private readonly string _routeBase;

		public FolderMiddleware(StorageFolder folder, string routeBase)
		{
			this._folder = folder;
			this._routeBase = routeBase;
		}

		public async Task Run(HttpContext context)
		{
			try
			{
				Regex regex = new Regex("^" + _routeBase);
				string url = context.Request.BaseUrl;
				url = regex.Replace(url, @"");

				if (url.Length > 0 && url[0] == '/') { url = url.Substring(1); }
				url = url.Replace('/', '\\');

				if (string.IsNullOrWhiteSpace(url)) { url = "index.html"; }

				StorageFile file = await this._folder.GetFileAsync(url);
				context.Response.Headers.Add("Content-type", file.ContentType);
				context.Response.Send(await ReadFile(file));
			}
			catch (Exception ex) when (ex is FileNotFoundException || ex is ArgumentException)
			{
				context.Response.StatusCode = HttpStatusCode.NotFound;
				context.Response.End();
			}
		}

		/// <summary>
		/// Loads the byte data from a StorageFile
		/// </summary>
		/// <param name="file">The file to read</param>
		public async Task<byte[]> ReadFile(StorageFile file)
		{
			using (IRandomAccessStreamWithContentType stream = await file.OpenReadAsync())
			{
				stream.Seek((ulong)0);
				byte[] fileBytes = new byte[stream.Size];
				var buffer = CryptographicBuffer.CreateFromByteArray(fileBytes);
				IBuffer rd = await stream.ReadAsync(buffer, (uint) fileBytes.Length, InputStreamOptions.None);
				rd.CopyTo(fileBytes);
				return fileBytes;
			}
		}
	}
}
