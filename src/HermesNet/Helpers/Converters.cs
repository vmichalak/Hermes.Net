using System.Collections.Generic;
using System.Collections.Specialized;

namespace HermesNet.Helpers
{
	internal class Converters
	{
		/// <summary>
		/// Converts a NameValueCollection to a string, string Dictionary
		/// </summary>
		/// <param name="source">The source NameValueCollection</param>
		/// <returns>The converted Dictionary</returns>
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
