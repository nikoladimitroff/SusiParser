using SusiParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SusiParsingService
{
	public class Logger
	{
		private StreamWriter writer;
		public Logger(string path)
		{
			writer = new StreamWriter(path);
			writer.AutoFlush = true;
			writer.WriteLine("<pre></pre>");
			writer.BaseStream.Seek("<pre>".Length, SeekOrigin.Begin);
		}

		public void LogLoginRequest(string username, string ip)
		{
			writer.WriteLine("[{0}] LOGIN: User [{1}] was just logged from address [{2}].", DateTime.UtcNow, username, ip);
		}

		public void LogCoursesRequest(string ip, CoursesTakenType type)
		{
			writer.WriteLine("[{0}] COURSES: Request about [{1}] courses served to address [{2}].", DateTime.UtcNow, type, ip);
		}

		public void LogStudentRequest(string ip)
		{
			writer.WriteLine("[{0}] STUDENT: Request about student information served to address [{1}]", DateTime.UtcNow, ip);
		}

		public void LogLogoutRequest(string key, string ip)
		{
			writer.WriteLine("[{0}] LOGOFF: User logged off and key [{1}] disposed due to request from [{2}]", DateTime.UtcNow, key, ip);
		}
	}
}