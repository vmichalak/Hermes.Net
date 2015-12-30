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
	public class HttpRequestTest
	{
		[TestMethod]
		public void NullParameterTest()
		{
			try
			{
				HttpRequest request = new HttpRequest("192.168.1.126", "/", "/", null, HttpMethod.POST, "");
				Assert.Fail();
			}
			catch (ArgumentNullException)
			{
				// test passed
			}
		}

		[TestMethod]
		public void InvalidHostTest()
		{
			try
			{
				HttpRequest request = new HttpRequest("abc", "/", "/", new Dictionary<string, string>(), HttpMethod.POST, "");
				Assert.Fail();
			}
			catch (ArgumentException)
			{
				// test passed
			}

			try
			{
				HttpRequest request = new HttpRequest("192.168.1.7893", "/", "/", new Dictionary<string, string>(), HttpMethod.POST, "");
				Assert.Fail();
			}
			catch (ArgumentException)
			{
				// test passed
			}
		}
		[TestMethod]
		public void AllGoodTest()
		{
			HttpRequest request = new HttpRequest("192.168.1.69", "/say?message=lol", "/say", new Dictionary<string, string> { { "message", "lol" } }, HttpMethod.POST, "");
			Assert.AreEqual(IPAddress.Parse("192.168.1.69"), request.Host);
			Assert.AreEqual("/say?message=lol", request.PathString);
			Assert.AreEqual(1, request.Parameters.Count);
			Assert.AreEqual(HttpMethod.POST, request.Method);
		}
	}
}
