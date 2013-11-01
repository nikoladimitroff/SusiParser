using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SusiParser
{
	/// <summary>
	/// Holds information about a single course.
	/// </summary>
	struct CourseInfo
	{
		public string CourseName;
		public string Teacher;
		public double Grade;
		public bool IsTaken;
		public bool IsElective;
		public double Credits;

		public override string ToString()
		{
			return string.Format("Course: {0}, Teacher: {1}, Grade: {2}, Credits: {3}, Elective: {4}, Taken: {5}", 
				this.CourseName, this.Teacher, this.Grade, this.Credits, this.IsElective, this.IsTaken);
		}
	}
}
