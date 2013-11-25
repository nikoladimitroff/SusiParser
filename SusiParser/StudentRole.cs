using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SusiParser
{
	[DataContract]
	public struct StudentRole
	{
		[DataMember(Name="roleText")]
		public string RoleText { get; private set; }
		[DataMember(Name="facultyNumber")]
		public string FacultyNumber { get; private set; }

		[IgnoreDataMember]
		public int HiddenUserId { get; private set; }
		[IgnoreDataMember]
		public int HiddenRoleId { get; private set; }

		public StudentRole(string roleText, string facultyNumber, int userId, int roleId) : 
			this()
		{
			this.RoleText = RoleText;
			this.FacultyNumber = facultyNumber;
			this.HiddenUserId = userId;
			this.HiddenRoleId = roleId;
		}
	}
}
