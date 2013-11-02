using SusiParser;
using SusiParsingService.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SusiParsingService.Controllers
{
    public class CoursesController : IPAwareApiController
    {
        // Post api/courses
        public IEnumerable<CourseInfo> Post([FromUri] int coursesType, [FromBody] KeyContainer keyContainer)
		{
			if (!Enum.IsDefined(typeof(CoursesTakenType), coursesType))
				throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Course Taken Type must be 0, 1 or 2"));

			string key = keyContainer.Key.Replace("\"", string.Empty);

			SusiParser.Parser parser;
			if (GlobalHost.Instance.TryGetValue(key, out parser))
			{
				try
				{
					IEnumerable<CourseInfo> info = parser.GetCourses((CoursesTakenType) coursesType);
					GlobalHost.Instance.Logger.LogCoursesRequest(this.GetClientIp(), (CoursesTakenType)coursesType);
					return info;
				}
				catch (WebException)
				{
					throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadGateway, "Can't load data from susi"));
				}
			}
			else
			{
				throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "No such key / key expired "));
			}

        }
    }
}
