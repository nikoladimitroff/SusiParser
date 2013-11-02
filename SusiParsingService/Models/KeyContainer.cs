using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace SusiParsingService.Models
{
	[DataContract]
	public class KeyContainer
	{
		[DataMember(Name = "key")]
		public string Key;

		public string GetNormalizedKey()
		{
			return Regex.Replace(this.Key, "\"|\'", string.Empty);
		}
	}
}