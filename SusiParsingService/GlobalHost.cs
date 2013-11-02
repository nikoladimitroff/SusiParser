using System.Collections.Concurrent;

using Parser = SusiParser.SusiParser;

namespace SusiParsingService
{
	public class GlobalHost
	{
		private static GlobalHost instance;

		public static GlobalHost Instance
		{
			get
			{
				if (instance == null)
					instance = new GlobalHost();

				return instance;
			}
		}

		private ConcurrentDictionary<string, Parser> parsers { get; set; }

		public GlobalHost()
		{
			this.parsers = new ConcurrentDictionary<string, Parser>();
		}

		public bool TryGetValue(string value, out Parser parser)
		{
			return this.parsers.TryGetValue(value, out parser);
		}

		public bool TryAdd(string key, Parser parser)
		{
			return this.parsers.TryAdd(key, parser);
		}
	}
}