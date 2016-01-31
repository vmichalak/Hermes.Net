using System;
using System.Threading.Tasks;
using HermesNet.Models.Http;

namespace HermesNet.Models
{
	internal class MiddlewareImplementator : IMiddleware
	{
		private readonly Func<HttpContext, Task> _func;

		public Task Run(HttpContext context)
		{
			return _func.Invoke(context);
		}

		public MiddlewareImplementator(Func<HttpContext, Task> func)
		{
			_func = func;
		}
	}
}
