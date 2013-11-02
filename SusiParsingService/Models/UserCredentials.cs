
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SusiParsingService.Models
{
	[DataContract]
	public class UserCredentials
	{
		[DataMember(Name = "username")]
		public string Username;
		[DataMember(Name = "password")]
		public string Password;
	}
}