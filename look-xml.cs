using System ;
using System.Data ;
using System.Xml ;


class Search
{
  public static void Main()
  {

   bool RecordFound = false ; // To check if the record exists.
   DataSet ds = new DataSet() ; // Creating a new DataSet to hold the XML Data.
   string result = "9";
   string filename = "C:\\barry\\software\\csharp\\employees.xml";
   Console.WriteLine("Read the XML file " + filename);
   ds.ReadXml(filename) ; // Getting the XML Data into the DataSet created.
   foreach(DataRow dr in ds.Tables[0].Rows)
   {
		// Checking if ID exists.
     if(result.Equals(dr["ID"]))
     {
		Console.WriteLine("");
		Console.WriteLine("Employee ID : " + dr["ID"]);
		Console.WriteLine("FirstName : " + dr["FirstName"]);
		Console.WriteLine("LastName : " + dr["LastName"]);
		Console.WriteLine("YearOfJoining : " + dr["YearOfJoining"]);
		Console.WriteLine("Department : " + dr["Department"]);

      RecordFound = true ; // Record Found
      break ; // No more search.
     } // if
   } // foreach
   if ( !RecordFound )
     Console.WriteLine("Record Does Not Exist !!!") ;
  } // end of Main
} // end of Search
