using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;

namespace HermesNet.Helpers
{
	internal class HttpUtility
	{
		public static NameValueCollection ParseQueryString(string queryString)
		{
			NameValueCollection nvc = new NameValueCollection();

			if (queryString.Contains('?'))
			{
				queryString = queryString.Substring(queryString.IndexOf('?') + 1);
			}

			if (string.IsNullOrWhiteSpace(queryString))
			{
				return nvc;
			}

			foreach (string[] singlePair in queryString.Split('&').Select(item => item.Split('=')))
			{
				nvc.Add(
					WebUtility.UrlDecode(singlePair[0]), 
					singlePair.Length == 2 ? WebUtility.UrlDecode(singlePair[1]) : string.Empty
				);
			}

			return nvc;
		} 
	}
}
