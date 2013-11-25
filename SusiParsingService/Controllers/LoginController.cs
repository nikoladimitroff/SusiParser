using SusiParser;
using SusiParsingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web.Http;

namespace SusiParsingService.Controllers
{
    public class LoginController : IPAwareApiController
    {
        // POST api/login
        public HttpResponseMessage Post([FromBody] UserCredentials credentials, [FromUri] bool selectFirstRole = true)
        {
			// Create a parser, try to log in
			Parser parser = new Parser();
			LoginResult result = parser.Login(credentials.Username, credentials.Password);
			// See if the result is not an error
			if (result == LoginResult.UnknownError)
			{
				throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadGateway, "Can't load data from susi"));
			}
			else if (result == LoginResult.InvalidCredentials)
			{
				throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid credentials!"));
			}

			// Add the parser with a new GUID as a key and log the request
			string key = Guid.NewGuid().ToString();
			GlobalHost.Instance.Add(key, parser);
			GlobalHost.Instance.Logger.LogLoginRequest(credentials.Username, this.GetClientIp());

			// Ok, everything went fine but our user is in multiple roles and we need to solve the ambiguity.
			// so return 300 and the roles that our user is in
			if (result == LoginResult.MultipleRoles)
			{
				if (!selectFirstRole)
				{
					return Request.CreateResponse<string>(HttpStatusCode.Ambiguous, key);
				}
				parser.ChangeRole(parser.Roles[0]);
			}

			// All is fine, return 200
			return Request.CreateResponse<string>(HttpStatusCode.OK, key);
        }

		// Delete api/login
		public HttpResponseMessage Delete([FromBody] KeyContainer keyContainer)
		{
			string key = keyContainer.Key.Replace("\"", string.Empty);
			if (GlobalHost.Instance.TryRemove(key))
			{
				GlobalHost.Instance.Logger.LogLogoutRequest(key, this.GetClientIp());

				return Request.CreateResponse(HttpStatusCode.OK);
			}
			return Request.CreateResponse(HttpStatusCode.NotFound);
		}

    }
}
