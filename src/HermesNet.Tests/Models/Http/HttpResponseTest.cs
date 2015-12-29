using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HermesNet.Models.Http;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace HermesNet.Tests.Models.Http
{
	[TestClass]
	public class HttpResponseTest
	{
		[TestMethod]
		public void AfterEndTest()
		{
			try
			{
				HttpResponse response = new HttpResponse();
				response.Send("Hello World");
				response.Send("Here you need to crash");
				Assert.Fail();
			}
			catch (UnauthorizedAccessException)
			{
				// test passed
			}

			try
			{
				HttpResponse response = new HttpResponse();
				response.End();
				response.Send("lol");
				Assert.Fail();
			}
			catch (UnauthorizedAccessException)
			{
				// test passed
			}

			try
			{
				HttpResponse response = new HttpResponse();
				response.End();
				response.StatusCode = HttpStatusCode.InternalServerError;
				Assert.Fail();
			}
			catch (UnauthorizedAccessException)
			{
				// test passed
			}
		}

		[TestMethod]
		public void ChangeStatusCodeTest()
		{
			HttpResponse response = new HttpResponse();
			response.StatusCode = HttpStatusCode.Accepted;
			Assert.AreEqual(HttpStatusCode.Accepted, response.StatusCode);
			response.StatusCode = HttpStatusCode.InternalServerError;
			Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
			response.StatusCode = HttpStatusCode.OK;
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
		}

		[TestMethod]
		public void SendBodyInBytesTest()
		{
			string body = "this is a unit test of body content.";
			byte[] bytes = Encoding.UTF8.GetBytes(body);
			HttpResponse response = new HttpResponse();
			response.Send(bytes);
			Assert.AreEqual(bytes, response.Body);
		}

		[TestMethod]
		public void SendBodyInStringTest()
		{
			string body = "test";
			byte[] bytes = Encoding.UTF8.GetBytes(body);
			HttpResponse response = new HttpResponse();
			response.Send(body);
			Assert.AreEqual(bytes.Length, response.Body.Length);
		}

		[TestMethod]
		public void EndedByEndTest()
		{
			HttpResponse response = new HttpResponse();
			response.End();
			Assert.AreEqual(true, response.Ended);
		}

		[TestMethod]
		public void EndedBySendTest()
		{
			HttpResponse response = new HttpResponse();
			response.Send("Hello guys !");
			Assert.AreEqual(true, response.Ended);
		}
	}
}
