using HermesNet.Models.Http;

namespace HermesNet.Models
{
	public interface IMiddleware
	{
		/// <summary>
		/// Running method of the Middleware
		/// </summary>
		/// <param name="context">The current context</param>
		void Run(HttpContext context);
	}
}
