using System.Collections.Generic;
using System.Collections.Specialized;

namespace HermesNet.Helpers
{
	internal class Converters
	{
		public static Dictionary<string, string> ConvertNameValueCollectionToDictionary(NameValueCollection source)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(source.Count);
			foreach (string key in source.AllKeys)
			{
				dictionary.Add(key, source[key]);
			}
			return dictionary;
		}
	}
}
