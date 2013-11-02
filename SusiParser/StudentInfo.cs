
using System.Runtime.Serialization;
namespace SusiParser
{
	[DataContract]
	public enum StudentType
	{
		Bachelor,
		Master,
		Doctor
	}

	[DataContract(Name = "student")]
	/// <summary>
	/// Holds information about a student. NOTE: In case the student has dropped but still has a SUSI account, his year and group will be 0
	/// </summary>
	public struct StudentInfo
	{
		[DataMember(Name = "firstName")]
		public string FirstName;
		[DataMember(Name = "middleName")]
		public string MiddleName;
		[DataMember(Name = "lastName")]
		public string LastName;
		[DataMember(Name = "facultyNumber")]
		public string FacultyNumber;
		[DataMember(Name = "programme")]
		public string Programme;
		[DataMember(Name = "type")]
		public StudentType Type;
		[DataMember(Name = "year")]
		public int Year;
		[DataMember(Name = "group")]
		public int Group;

		public override string ToString()
		{
			return string.Format("Name: {0}, FN: {1}, Programme: {2}, Year: {3}, Group: {4}, Type: {5}",
				FirstName + " " + MiddleName + " " + LastName,
				FacultyNumber,
				Programme,
				Year,
				Group,
				Type);
		}
	}
}
