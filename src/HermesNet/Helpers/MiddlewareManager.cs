using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HermesNet.Models;
using HermesNet.Models.Http;

namespace HermesNet.Helpers
{
	internal class MiddlewareManager
	{
		private class Entry
		{
			public HttpMethod Method { get; set; }
			public string Route { get; set; }
		}

		private readonly Dictionary<Entry, IMiddleware> _middlewares = new Dictionary<Entry, IMiddleware>();

		public void Add(string route, HttpMethod method, IMiddleware middleware)
		{
			this._middlewares.Add(new Entry() { Method = method, Route = route }, middleware);
		}

		public async Task<HttpResponse> Execute(HttpRequest request)
		{
			HttpContext context = new HttpContext(request);

			try
			{
				Entry searchEntry = new Entry()
				{
					Method = request.Method,
					Route = request.BaseUrl
				};

				IMiddleware middleware = FilterMiddlewares(searchEntry);
				if (middleware == null)
				{
					context.Response.StatusCode = HttpStatusCode.NotFound;
				}
				else
				{
					await middleware.Run(context);
				}
			}
			catch (Exception e)
			{
#if !DEBUG
				context.Response.StatusCode = HttpStatusCode.InternalServerError;
				context.Response.Send("Internal Server Error: " + e.Message);
#else
				throw new Exception("Error during MiddlewareManager Execution.", e);
#endif
			}

			return context.Response;
		}

		private IMiddleware FilterMiddlewares(Entry searchEntry)
		{
			return this._middlewares.FirstOrDefault(m =>
			{
				Regex regex = new Regex("^"+m.Key.Route);
				return (m.Key.Method == HttpMethod.ALL || m.Key.Method == searchEntry.Method) && regex.IsMatch(searchEntry.Route);
			}).Value;
		}
	}
}
