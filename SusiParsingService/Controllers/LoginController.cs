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
    public class LoginController : ApiController
    {
        // POST api/login
        public string Post([FromBody] UserCredentials credentials)
        {
			string key = Guid.NewGuid().ToString();
			SusiParser.SusiParser parser = new SusiParser.SusiParser();
			try 
			{	        
				parser.Login(credentials.Username, credentials.Password);
				if (GlobalHost.Instance.TryAdd(key, parser))
				{
					return key;
				}
				else
				{
					throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Service unavailable"));
				}
			}
			catch (InvalidCredentialException)
			{
				throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid credentials!"));
			}
			catch (WebException)
			{
				throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Can't load data from susi"));
			}
        }

    }
}
