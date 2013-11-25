using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SusiParser
{
	public struct StudentRole
	{
		public string RoleText { get; private set; }
		public string FacultyNumber { get; private set; }
		public int HiddenUserId { get; private set; }
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
