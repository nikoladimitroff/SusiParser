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
        public string Post([FromBody] UserCredentials credentials)
        {
			string key = Guid.NewGuid().ToString();
			SusiParser.Parser parser = new SusiParser.Parser();
			try 
			{	        
				parser.Login(credentials.Username, credentials.Password);
				if (GlobalHost.Instance.TryAdd(key, parser))
				{
					GlobalHost.Instance.Logger.LogLoginRequest(credentials.Username, this.GetClientIp());
					return key;
				}
				else
				{
					var response = Request.CreateErrorResponse(HttpStatusCode.ServiceUnavailable, "Service unavailable");
					
					throw new HttpResponseException(response);
				}
			}
			catch (InvalidCredentialException)
			{
				throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid credentials!"));
			}
			catch (WebException)
			{
				throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadGateway, "Can't load data from susi"));
			}
        }

		// Delete api/login
		public void Delete([FromBody] string key)
		{
			key = key.Replace("\"", string.Empty);
			if (GlobalHost.Instance.TryRemove(key))
			{
				GlobalHost.Instance.Logger.LogLogoutRequest(key, this.GetClientIp());
			}
		}

    }
}
