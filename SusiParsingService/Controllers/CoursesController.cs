using SusiParser;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SusiParsingService.Controllers
{
    public class CoursesController : ApiController
    {
        // GET api/courses
        public IEnumerable<CourseInfo> Post([FromUri] int coursesType, [FromBody]string value)
		{
			if (!Enum.IsDefined(typeof(CoursesTakenType), coursesType))
				throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Course Taken Type must be 0, 1 or 2"));

			value = value.Replace("\"", string.Empty);

			SusiParser.SusiParser parser;
			if (GlobalHost.Instance.TryGetValue(value, out parser))
			{
				try
				{
					IEnumerable<CourseInfo> info = parser.GetCourses((CoursesTakenType) coursesType);
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
