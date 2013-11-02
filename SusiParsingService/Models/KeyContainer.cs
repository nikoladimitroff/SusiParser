using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SusiParsingService.Models
{
	[DataContract]
	public class KeyContainer
	{
		[DataMember(Name = "key")]
		public string Key;
	}
}