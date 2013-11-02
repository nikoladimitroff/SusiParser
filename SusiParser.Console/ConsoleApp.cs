using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace SusiParser
{
	class Program
	{
		static void Main(string[] args)
		{
			//string address = @"http://susi.apphb.com/api";
			string address = "http://localhost:61655/api";
			string login = @"/login";
			string student = @"/student";
			string courses = @"/courses";

			Console.WriteLine("Enter your credentials");
			Console.Write("Username: ");
			string username = Console.ReadLine();
			Console.Write("Password: ");
			string password = Console.ReadLine();

			WebResponse response = CreateRequest("POST", address + login, new { username = username, password = password });
			string key;
			using (Stream stream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
			{
				key = reader.ReadToEnd().Replace("\"", string.Empty);
			}

			Console.WriteLine("Your key to the service is {0}", key);

			response = CreateRequest("POST", address + student, new { key = key });
			ReadResponse(response, "STUDENT INFORMATION");

			response = CreateRequest("POST", address + courses + "?coursesType=0", new { key = key });
			ReadResponse(response, "COURSE INFORMATION");
		}

		private static void ReadResponse(WebResponse response, string title)
		{
			string info;
			using (Stream stream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
			{
				info = reader.ReadToEnd();
			}
			Console.WriteLine("{0}{1}{0}\r\n{2}", new string('*', 10), title, info);
		}

		private static WebResponse CreateRequest(string method, string address, object data)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
			request.Method = method;
			request.ContentType = "application/json";
			if (method == "POST")
			{
				using (var requestStream = request.GetRequestStream())
				using (var writer = new StreamWriter(requestStream))
				{
					writer.Write(JsonConvert.SerializeObject(data));
					Debug.WriteLine(JsonConvert.SerializeObject(data));
				}
			}
			return request.GetResponse();
		}
	}
}
