namespace HermesNet.Models.Http
{
	public class HttpContext
	{
		public HttpRequest Request { get; }

		public HttpResponse Response { get; }

		public HttpContext(HttpRequest request)
		{
			this.Request = request;
			this.Response = new HttpResponse();
		}
	}
}
