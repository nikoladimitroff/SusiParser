using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SusiParser
{
	class SusiParser
	{
		static string susiAddress = @"https://susi.uni-sofia.bg";
		static string loginPageAddress = @"https://susi.uni-sofia.bg/ISSU/forms/Login.aspx";
		static string homePageAddress = @"https://susi.uni-sofia.bg/ISSU/forms/students/home.aspx";
		static string reportExamsAddress = @"https://susi.uni-sofia.bg/ISSU/forms/students/ReportExams.aspx";

		public CookieContainer Cookies = new CookieContainer();

		public void Login(string user, string password)
		{
			ServicePointManager.Expect100Continue = false;

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(loginPageAddress);
			request.CookieContainer = this.Cookies;
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			using (var requestStream = request.GetRequestStream())
			using (var writer = new StreamWriter(requestStream))
			{
				// SQL INJECTION TIME!
				string data = string.Format("__EVENTTARGET=&__EVENTARGUMENT=&__VSTATE=eJz7z8ifws%252fKZWhsamBhYWBgYsmfIsaUhkKIMDHyizHJsYdlFmcm5aRmpDAxA%252fnyDEAGK0gNUBokzxKSWlGSmpLCxI4QlGcECXCiC3CjC%252fDCDORHlxGEyQjzQ1kpALbbHB0%253d&__VIEWSTATE=&txtUserName={0}&txtPassword={1}&btnSubmit=%D0%92%D0%BB%D0%B5%D0%B7&__EVENTVALIDATION=%2FwEWBAL%2BraDpAgKl1bKzCQK1qbSRCwLCi9reA3ZPH%2F0OXuiqks41bB%2BF30DwDzP9", user, password);
				writer.Write(data);
			}

			WebResponse response = request.GetResponse();
		}

		public string GetExamPage()
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(reportExamsAddress);
			request.CookieContainer = this.Cookies;
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			using (var requestStream = request.GetRequestStream())
			using (var writer = new StreamWriter(requestStream))
			{
				writer.Write("__EVENTTARGET=Report_Exams1%24btnReportExams&__EVENTARGUMENT=&__VSTATE=eJytUs1u00AQjjd2SkKRQaAILomRciRJnUMlyi0tqBU%252fQqXiarn1prGS2MFeKwVxCOHvBZA4gYTEA4SqEVabnwfgMpZ4H5jdbgpKr5Xs9cw345nvm9k%252fin5T0QqWte57LPDb4TZ9EbkBfeqHrG7vtR7Sl5aVT2ulTWo7NHhMvciU9iPaYGZpq2Pv03rEmO%252bZWn6bdv2AWfcP7E5olvaarR27RT3t1jn8ic9ESNdyZq1WM1dqK6urupMnDTxyRMHzEjcJIeJUdD1PCuoOPWDaBnyHIRwnb%252bEkGcAoGRgwg6kBYxhyX35jOIahATE%252bMEnewTTp%252fx9xHNEkI0%252bC9dViSluGL5jzEeufwLSw9NwN3d02bWK2KjOU10WF%252bxlBqBnQhna3yVh3rVrt9XqVRsetRJ5bDv2Ga1d296sd24vsdlgNo9CtNmm7W35ll0MWOdRjtkPSXBiWvQrfkgFKOkIBk2SwZsiWPFZBUpz0FEmhpE%252fojCHG9D5Ch%252bcASY7TJEuS9T34jJpmmPQGDtGawMiAn2JA%252bPkhpjPhwzriKckHhGfYLebgqV6%252bB1XsIYP1NuErJiFVpBXfMXAbffT4zwMxciN5L2oO8Y1R1pijfEvYn29ohE043biQfeAHdN1v%252b0EuVVCt28%252fqRBXapYL0%252fFbcwOZnmDS0f5ejmKIO17sAZBeBy4vAlXkxfTFybR65rkvLITlxEbNygVsb5NfvHMLLYjqLd0S7qOn%252fBecwaV0%253d&__VIEWSTATE=&Report_Exams1%24chkTaken=on&__EVENTVALIDATION=%2FwEWGAL%2BraDpAgK3pdr8BQKN%2BNnHCgKJ7OiyDgL77521DgLy1aS1CQK%2FqPbTAQLVh87yAgLInunZCAKtla77CAL%2BsbCuCgKd6OrBDQKOiLS8DALkxb3FBwKOy6myDQLrn7D%2BBgKmzenZCALztLy4DgKTxe2tBwLHieKTBgKS4YnZCwKu3dGoAQKZ3sM%2BAt%2FH5N8HqzstQIPHe%2F5TQABqakO68cYBFzo%3D");
			}

			WebResponse response = request.GetResponse();

			using (var responseStream = response.GetResponseStream())
			using (var reader = new StreamReader(responseStream))
			{
				var result = reader.ReadToEnd();
				return result;
			}
		}
	}
}
