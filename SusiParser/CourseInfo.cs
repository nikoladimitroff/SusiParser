using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SusiParser
{
	[DataContract(Name = "course")]
	/// <summary>
	/// Holds information about a single course.
	/// </summary>
	public struct CourseInfo
	{
		[DataMember(Name = "name")]
		public string CourseName;
		[DataMember(Name = "teacher")]
		public string Teacher;
		[DataMember(Name = "grade")]
		public double Grade;
		[DataMember(Name = "isTaken")]
		public bool IsTaken;
		[DataMember(Name = "isElective")]
		public bool IsElective;
		[DataMember(Name = "credits")]
		public double Credits;

		public override string ToString()
		{
			return string.Format("Course: {0}, Teacher: {1}, Grade: {2}, Credits: {3}, Elective: {4}, Taken: {5}", 
				this.CourseName, this.Teacher, this.Grade, this.Credits, this.IsElective, this.IsTaken);
		}
	}
}
