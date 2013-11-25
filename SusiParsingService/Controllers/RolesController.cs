using SusiParser;
using SusiParsingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SusiParsingService.Controllers
{
    public class RolesController : IPAwareApiController
    {
		// I know, I know post should not be used for getting stuff but fuck it
        // POST api/roles
        public IEnumerable<StudentRole> Post([FromBody] KeyContainer key)
        {
			Parser parser;
			if (GlobalHost.Instance.TryGetValue(key.GetNormalizedKey(), out parser))
			{
				return parser.Roles;
			}

			// If the key was not found
			throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "No such key / key expired "));
        }

        // PUT api/roles
        public void Put([FromUri] int roleIndex, [FromBody]KeyContainer key)
		{
			Parser parser;
			if (GlobalHost.Instance.TryGetValue(key.GetNormalizedKey(), out parser))
			{
				parser.ChangeRole(parser.Roles[roleIndex]);
				return;
			}

			// If the key was not found
			throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "No such key / key expired "));
        }

    }
}
