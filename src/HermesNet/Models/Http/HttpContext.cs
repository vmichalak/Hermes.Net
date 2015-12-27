using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
