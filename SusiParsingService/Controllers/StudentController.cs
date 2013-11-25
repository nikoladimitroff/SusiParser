using SusiParser;
using SusiParsingService.Models;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace SusiParsingService.Controllers
{
    public class StudentController : IPAwareApiController
    {
        // POST api/student
        public StudentInfo Post([FromBody] KeyContainer keyContainer)
		{
			string key = keyContainer.GetNormalizedKey();
			
			SusiParser.Parser parser;
			if (GlobalHost.Instance.TryGetValue(key, out parser))
			{
				try
				{
					StudentInfo info = parser.GetStudentInfo();
					GlobalHost.Instance.Logger.LogStudentRequest(this.GetClientIp());
					return info;
				}
				catch (WebException)
				{
					throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadGateway, "Can't load data from susi"));
				}
			}

			// If the key was not found
			throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "No such key / key expired "));
        }
    }
}
