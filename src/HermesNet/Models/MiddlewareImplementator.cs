using System;
using System.Threading.Tasks;
using HermesNet.Models.Http;

namespace HermesNet.Models
{
	internal class MiddlewareImplementator : IMiddleware
	{
		private readonly Action<HttpContext> _action;

		public Task Run(HttpContext context)
		{
			return Task.Run(() => _action(context));
		}

		public MiddlewareImplementator(Action<HttpContext> action)
		{
			_action = action;
		}
	}
}
