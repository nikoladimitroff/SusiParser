using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading;
using System.Web;

namespace SusiParser
{
	public enum CoursesTakenType
	{
		All,
		Taken,
		NotTaken
	}

	/// <summary>
	/// A class mighty enought to break trough SUSI, hooray!
	/// </summary>
	public class Parser
	{
		static class SusiPages
		{
			public static string SusiAddress = @"https://susi.uni-sofia.bg";
			public static string LoginPageAddress = @"https://susi.uni-sofia.bg/ISSU/forms/Login.aspx";
			public static string HomePageAddress = @"https://susi.uni-sofia.bg/ISSU/forms/students/home.aspx";
			public static string ReportExamsAddress = @"https://susi.uni-sofia.bg/ISSU/forms/students/ReportExams.aspx";
		}

		// Let the magic begin!
		static class HiddenFieldValues
		{
			public static string NotTakenCoursesFlag = "&Report_Exams1%24chkNotTaken=on";
			public static string TakenCoursesFlag = "&Report_Exams1%24chkTaken=on";
			public static string FormattedHiddenFields = "__EVENTTARGET=Report_Exams1%24btnReportExams&__EVENTARGUMENT=&__VSTATE={0}&__VIEWSTATE=&__EVENTVALIDATION=%2FwEWGAL%2BraDpAgK3pdr8BQKN%2BNnHCgKJ7OiyDgL77521DgLy1aS1CQK%2FqPbTAQLVh87yAgLInunZCAKtla77CAL%2BsbCuCgKd6OrBDQKOiLS8DALkxb3FBwKOy6myDQLrn7D%2BBgKmzenZCALztLy4DgKTxe2tBwLHieKTBgKS4YnZCwKu3dGoAQKZ3sM%2BAt%2FH5N8HqzstQIPHe%2F5TQABqakO68cYBFzo%3D";
			public static string HardcodedHiddenFields = "__EVENTTARGET=Report_Exams1%24btnReportExams&__EVENTARGUMENT=&__VSTATE=eJz7z8ifws%2fKZWhsamBhYWBgYsmfIsaUhkKIMDHyizHJsYdlFmcm5aRmpDAxA%2fnyDEAGK0gNUBokzxKSWlGSmpLCxI4QlGcECXCiC3CjC%2fDCDORHlxGEyQjzQ1kpALbbHB0%3d&__VIEWSTATE=&Report_Exams1%24chkTaken=on&__EVENTVALIDATION=%2FwEWGAL%2BraDpAgK3pdr8BQKN%2BNnHCgKJ7OiyDgL77521DgLy1aS1CQK%2FqPbTAQLVh87yAgLInunZCAKtla77CAL%2BsbCuCgKd6OrBDQKOiLS8DALkxb3FBwKOy6myDQLrn7D%2BBgKmzenZCALztLy4DgKTxe2tBwLHieKTBgKS4YnZCwKu3dGoAQKZ3sM%2BAt%2FH5N8HqzstQIPHe%2F5TQABqakO68cYBFzo%3D";
			public static string FormattedLoginFormData = "__EVENTTARGET=&__EVENTARGUMENT=&__VSTATE=eJz7z8ifws%252fKZWhsamBhYWBgYsmfIsaUhkKIMDHyizHJsYdlFmcm5aRmpDAxA%252fnyDEAGK0gNUBokzxKSWlGSmpLCxI4QlGcECXCiC3CjC%252fDCDORHlxGEyQjzQ1kpALbbHB0%253d&__VIEWSTATE=&txtUserName={0}&txtPassword={1}&btnSubmit=%D0%92%D0%BB%D0%B5%D0%B7&__EVENTVALIDATION=%2FwEWBAL%2BraDpAgKl1bKzCQK1qbSRCwLCi9reA3ZPH%2F0OXuiqks41bB%2BF30DwDzP9";
		}

		public CookieContainer Cookies = new CookieContainer();

		private string vstate = string.Empty;

		public Parser()
		{
			// Remove the form flag, otherwise the html parser will set the children of any <form> as its siblings because fuck logic
			HtmlNode.ElementsFlags.Remove("form");
		}

		private WebResponse SendRequest(string address, string data)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
			request.CookieContainer = this.Cookies;
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			using (var requestStream = request.GetRequestStream())
			using (var writer = new StreamWriter(requestStream))
			{
				writer.Write(data);
			}
			return request.GetResponse();
		}

		/// <summary>
		/// Extracts the current value of the _VSTATE hidden field.
		/// </summary>
		/// <param name="response">A web response containing the page to extract the field from.</param>
		private void ExtractVstate(WebResponse response)
		{
			HtmlDocument document = new HtmlDocument();
			document.Load(response.GetResponseStream());

			// Magical xpath does wonders
			string vstateXpath = @"//input[@id=""__VSTATE""]";
			HtmlNode node = document.DocumentNode.SelectSingleNode(vstateXpath);
			this.vstate = node.Attributes["value"].Value.Trim();
		}

		private string GetHomePage()
		{
			// As above, send a post request with predefined values for the hidden fields
			WebResponse response = this.SendRequest(SusiPages.HomePageAddress, HiddenFieldValues.HardcodedHiddenFields);

			using (var responseStream = response.GetResponseStream())
			using (var reader = new StreamReader(responseStream))
			{
				var result = reader.ReadToEnd();
				return result;
			}
		}

		private string GetExamPage(CoursesTakenType coursesType, bool shouldRecurse = true)
		{
			// AWFUL CODE, BLAME SUSI

			// Send a post request with predefined values of the hidden fields
			string data = string.Format(HiddenFieldValues.FormattedHiddenFields, HttpUtility.UrlEncode(this.vstate));
			switch (coursesType)
			{
				case CoursesTakenType.NotTaken:
					data += HiddenFieldValues.NotTakenCoursesFlag;
					break;
				case CoursesTakenType.Taken:
					data += HiddenFieldValues.TakenCoursesFlag;
					break;
				default:
					data += HiddenFieldValues.TakenCoursesFlag + HiddenFieldValues.NotTakenCoursesFlag;
					break;

			}
			WebResponse response = this.SendRequest(SusiPages.ReportExamsAddress, data);

			// Since SUSI sucks, in order to get the courses page we first need the value of a special hidden field - _VSTATE.
			// Unfortunately, _VSTATE has different value on other pages, so first need to get its value from the current response,
			// Then send another request with the updated value (which is what the recursion is for)
			if (shouldRecurse)
			{
				ExtractVstate(response);
				return this.GetExamPage(coursesType, false);
			}

			// If everything else is ok return the whole page
			using (var responseStream = response.GetResponseStream())
			using (var reader = new StreamReader(responseStream))
			{
				var result = reader.ReadToEnd();
				return result;
			}
		}

		/// <summary>
		/// Logs the user in SUSI.
		/// </summary>
		/// <param name="username">The username in Susi</param>
		/// <param name="password">The password in Susi</param>
		public void Login(string username, string password)
		{
			try
			{
				ServicePointManager.Expect100Continue = false;

				WebResponse response = this.SendRequest(SusiPages.LoginPageAddress, string.Format(HiddenFieldValues.FormattedLoginFormData, username, password));
				// If susi has returned the same page, the credentials are wrong
				if (response.ResponseUri.ToString() == SusiPages.LoginPageAddress)
				{
					throw new InvalidCredentialException("Invalid credentials");
				}
				this.ExtractVstate(response);
			}
			catch (WebException e)
			{
				throw new WebException("Can't log into SUSI. (Most likely SUSI can't be reached due to downtime)", e);
			}
		}

		/// <summary>
		/// Gets a collection of <see cref="CourseInfo"/> that shows all courses that the student has passed successfully.
		/// </summary>
		public IEnumerable<CourseInfo> GetCourses(CoursesTakenType coursesType)
		{
			if (this.Cookies.Count == 0)
			{
				throw new InvalidOperationException("You must login prior to getting info from SUSI");
			}
#if !DEBUG
			try
			{
#endif
			// Get the exam page and parse it
			string examPage = this.GetExamPage(coursesType);
			HtmlDocument document = new HtmlDocument();
			document.LoadHtml(examPage);

			// Use a magical xpath to get all rows of the courses table, there may be header rows though
			string xpath = @"/html/body/form/table/tr/td/table[@width=""100%""]/tr/td/table/tr[not(@class)]";
			var nodes = document.DocumentNode.SelectNodes(xpath);

			// Set the culture to BG, else parsing doubles will fail
			CultureInfo culture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("bg-BG");

			List<CourseInfo> courseInfos = new List<CourseInfo>();
			foreach (HtmlNode node in nodes)
			{
				// Each tr has 6 tds inside in the following order:
				// Subject Teacher Type(elective or not) Taken Grade Credits
				HtmlNodeCollection children = node.SelectNodes("td");

				// If the first cell contains the following magic word, the row is a header, ignore it
				if (children[0].InnerText.Contains("Предмет"))
					continue;

				CourseInfo courseInfo = ExtractCourseInfo(children);

				courseInfos.Add(courseInfo);
			}
			// Set the culture back to whatever it was
			Thread.CurrentThread.CurrentCulture = culture;

			return courseInfos;
#if !DEBUG
			}
			catch (Exception e)
			{
				// In case something failed, throw a new exception
				throw new WebException("Can't load data from SUSI", e);
			}
#endif
		}

		private static CourseInfo ExtractCourseInfo(HtmlNodeCollection children)
		{
			// Make sure to strip away all whitespaces (trim everything)
			CourseInfo courseInfo = new CourseInfo();
			courseInfo.CourseName = children[0].InnerText.Trim();
			courseInfo.Teacher = children[1].InnerText.Trim();
			courseInfo.IsElective = children[2].InnerText.Trim() != "Задължителни";

			// The text in Grade and IsTaken is put inside a span unlike all the other cells, so find the first span child
			courseInfo.IsTaken = children[3].SelectSingleNode("span").InnerText.Trim() == "да";
			// If the grade is empty, set it to 0 (exam may have not yet passed)
			string gradeAsText = children[4].SelectSingleNode("span").InnerText.Trim();
			if (gradeAsText.Length != 0)
			{
				courseInfo.Grade = Double.Parse(gradeAsText.Substring(0, 1));
			}
			else
			{
				courseInfo.Grade = 0;
			}
			courseInfo.Credits = Double.Parse(children[5].InnerText.Trim());
			return courseInfo;
		}

		/// <summary>
		/// Gets information about the currently logged in student from Susi.
		/// </summary>
		public StudentInfo GetStudentInfo()
		{
			if (this.Cookies.Count == 0)
			{
				throw new InvalidOperationException("You must login prior to getting info from SUSI");
			}

#if !DEBUG
			try
			{
#endif
			// AVADA KEDAVRA, DIE SUSI!
			HtmlDocument document = new HtmlDocument();
			document.LoadHtml(this.GetHomePage());

			string xpath = @"html/body/form/table[3]/tr/td[2]";
			HtmlNode node = document.DocumentNode.SelectSingleNode(xpath);

			// That table cell we just selected has a quite an interesting structure:
			// - some of the text is plain text put right inside the cell
			// - other parts are inside a span
			// - other parts are inside a span inside a span
			// - lastly, there are parts inside a span inside a strong

			StudentInfo info = new StudentInfo();
			// The name is inside span inside span
			string[] names = node.SelectSingleNode("span/span").InnerText.Trim().Split(' ');
			info.FirstName = names[0];
			info.MiddleName = names[1];
			info.LastName = names[2];

			// The other elements are inside span inside strong
			HtmlNodeCollection otherInfo = node.SelectNodes("strong/span");
			info.FacultyNumber = otherInfo[0].InnerText.Trim();
			info.Programme = otherInfo[1].InnerText.Trim();
			// The following is true in case the student has dropped (the base)
			if (string.IsNullOrWhiteSpace(otherInfo[2].InnerHtml))
			{
				info.Year = 0;
				info.Group = 0;
			}
			else
			{
				info.Year = Int32.Parse(otherInfo[2].InnerText.Replace("Курс", "").Trim());
				info.Group = Int32.Parse(otherInfo[3].InnerText.Replace("Група", "").Trim());
			}
			// I am too lazy to split this in temporary variables but the idea is to read a special word appended after the current year in SUSI
			string studentType = node.SelectNodes("span").Last().InnerText.Split(',')[1].Trim().ToUpperInvariant();

			// Finally, the last of 'em magic stringz.
			// Check what we just got from the long line above for matches
			StudentType type;
			if (studentType == "БАКАЛАВРИ")
				type = StudentType.Bachelor;
			else if (studentType == "МАГИСТРИ")
				type = StudentType.Master;
			else
				type = StudentType.Doctor;

			info.Type = type;

			return info;

#if !DEBUG
			}
			catch (Exception e)
			{
				// In case something failed, throw a new exception
				throw new WebException("Can't load data from SUSI", e);
			}
#endif
		}
	}
}
