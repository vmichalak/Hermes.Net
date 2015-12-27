using System.Collections.Generic;
using HermesNet.Models;
using HermesNet.Models.Http;

namespace HermesNet.Helpers
{
	internal class MiddlewareManager
	{
		private readonly List<IMiddleware> _middlewares = new List<IMiddleware>();

		public void Add(IMiddleware middleware)
		{
			this._middlewares.Add(middleware);
		}

		public HttpResponse Execute(HttpRequest request)
		{
			HttpContext context = new HttpContext(request);

			foreach (IMiddleware middleware in this._middlewares)
			{
				middleware.Run(context);
				if (context.Response.Ended) { break; }
			}

			return context.Response;
		}
	}
}
