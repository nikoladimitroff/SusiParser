
namespace SusiParser
{
	public enum StudentType
	{
		Bachelor,
		Master,
		Doctor
	}

	/// <summary>
	/// Holds information about a student. NOTE: In case the student has dropped but still has a SUSI account, his year and group will be 0
	/// </summary>
	public struct StudentInfo
	{
		public string FirstName;
		public string MiddleName;
		public string LastName;
		public string FacultyNumber;
		public string Programme;
		public StudentType Type;
		public int Year;
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
