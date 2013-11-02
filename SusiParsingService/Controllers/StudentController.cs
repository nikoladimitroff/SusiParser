using SusiParser;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SusiParsingService.Controllers
{
    public class StudentController : IPAwareApiController
    {
        // POST api/student
        public StudentInfo Post([FromBody]string value)
		{
			value = value.Replace("\"", string.Empty);
			
			SusiParser.Parser parser;
			if (GlobalHost.Instance.TryGetValue(value, out parser))
			{
				try
				{
					StudentInfo info = parser.GetStudentInfo();
					GlobalHost.Instance.Logger.LogStudentRequest(this.GetClientIp());
					return info;
				}
				catch (WebException)
				{
					throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Can't load data from susi"));
				}
			}
			else
			{
				throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "No such key / key expired "));
			}

        }
    }
}
