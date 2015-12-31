namespace HermesNet.Models.Http
{
	public class HttpContext
	{
		/// <summary>
		/// Contain all incoming requests informations
		/// </summary>
		public HttpRequest Request { get; }

		/// <summary>
		/// Contain all outgoing requests informations
		/// </summary>
		public HttpResponse Response { get; }

		public HttpContext(HttpRequest request)
		{
			this.Request = request;
			this.Response = new HttpResponse();
		}
	}
}
