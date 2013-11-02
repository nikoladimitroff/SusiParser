using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Web.Hosting;
using Parser = SusiParser.Parser;

namespace SusiParsingService
{
	public class GlobalHost
	{
		private static GlobalHost instance;
		private static readonly int MaxParsers = 100000;
		private static readonly int ParsersToRemoveOnOverflow = 10000;

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
		private ConcurrentDictionary<string, DateTime> parserAccessDates { get; set; }

		public Logger Logger { get; private set; }

		public GlobalHost()
		{
			this.parsers = new ConcurrentDictionary<string, Parser>();
			this.parserAccessDates = new ConcurrentDictionary<string, DateTime>();
			this.Logger = new Logger(HostingEnvironment.MapPath("~/Log.html"));
		}

		public bool TryGetValue(string key, out Parser parser)
		{
			if (this.parsers.TryGetValue(key, out parser))
			{
				this.parserAccessDates[key] = DateTime.UtcNow;
				return true;
			}
			return false;
		}

		public bool TryAdd(string key, Parser parser)
		{
			if (this.parsers.TryAdd(key, parser))
			{
				this.parserAccessDates[key] = DateTime.UtcNow;
				// If we exceed the maximum amount of parsers, remove the oldest ones 
				if (this.parsers.Count > GlobalHost.MaxParsers)
				{
					var toBeRemoved = this.parserAccessDates.OrderBy(x => x.Value).Take(GlobalHost.ParsersToRemoveOnOverflow).ToList();
					DateTime dummyDateTime;
					Parser dummyParser;
					foreach (var entry in toBeRemoved)
					{
						this.parserAccessDates.TryRemove(entry.Key, out dummyDateTime);
						this.parsers.TryRemove(entry.Key, out dummyParser);
					}
				}
				return true;
			}
			return false;
		}

		public void TryRemove(string key)
		{
			DateTime dummyDateTime;
			Parser dummyParser;

			this.parsers.TryRemove(key, out dummyParser);
			this.parserAccessDates.TryRemove(key, out dummyDateTime);
		}
	}
}