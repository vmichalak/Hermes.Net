using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
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
				Regex regex = new Regex(_routeBase);
				string url = context.Request.BaseUrl;
				url = regex.Replace(url, @"");
				if (url[0] == '/')
				{
					url = url.Substring(1);
				}
				Debug.WriteLine("fileurl: "+url);
				StorageFile file = await this._folder.GetFileAsync(url);
				context.Response.Send(await ReadFile(file));
			}
			catch (FileNotFoundException)
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
			byte[] fileBytes = null;
			using (IRandomAccessStreamWithContentType stream = await file.OpenReadAsync())
			{
				stream.Seek((ulong)0);
				Debug.WriteLine("size: "+stream.Size);
				fileBytes = new byte[stream.Size];
				var buffer = Windows.Security.Cryptography.CryptographicBuffer.CreateFromByteArray(fileBytes);
				IBuffer rd = await stream.ReadAsync(buffer, (uint) fileBytes.Length, InputStreamOptions.None);
				rd.CopyTo(fileBytes);
			}
			Debug.WriteLine(fileBytes);
			return fileBytes;
		}
	}
}
