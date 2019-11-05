//////////////////////////////////////////////////////////////////////
//
// File      : mysql-cgi.cs
//
// Author    : Barry Kimelman
//
// Created   : November 3, 2019
//
// Purpose   : CGI/MySQL Test
//
// Notes     : (none)
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ConsoleApplication1 {
	class ConsoleApplication1 {
		static string input_data = "";
		static int DataLength;
		static int debug_mode = 0;
		static int max_post_length = 2048;
		static char[] SplitChars1 = new char[]{'&'} ;
		static char[] SplitChars2 = new char[]{'='} ;
		static int num_vars;
		static string[] input_fields;
		static Dictionary<string, string> data_fields = new Dictionary<string, string>();
		static string top_level_href = "<A HREF='mysql-cgi.exe'>Back to Top</A>";
		static string title_string = "C# MySQL CGI Shell";

		static public void debug_print(string message)
		{
			if ( debug_mode != 0 ) {
				Console.WriteLine(message);
			}
			return;
		}
		static public void parse_input_fields()
		{
			debug_print("<H4>Parse the " + DataLength + " bytes of input_data</H4>");
			if ( DataLength == 0 ) {
				num_vars = 0;
				debug_print("<br><h3>number of input fields is zero</h3>");
			}
			else {
				input_fields = input_data.Split(SplitChars1) ;
				num_vars = input_fields.Length;
				debug_print("<br><BR>number of input fields = " + num_vars);
				for ( int var_index = 0 ; var_index < num_vars ; ++var_index ) {
					string[] parts = input_fields[var_index].Split(SplitChars2) ;
					int num_parts = parts.Length;
					debug_print("<h4>name = " + parts[0] + " , value = " + parts[1] +"</H4>");
					data_fields.Add(parts[0], parts[1]);
				}
			}

			return;
		} // end of parse_input_fields

		static public void generate_choose_schema()
		{
			int count;
			string schema;
			string cs = @"server=localhost;userid=root;
				password=mypassword;database=myschema";
			Console.WriteLine("<H3>Choose one of the following schemas</H3>");

			MySqlConnection conn = null;
			MySqlDataReader rdr = null;
			count = 0;
			try 
			{
				conn = new MySqlConnection(cs);
				conn.Open();
        
				string query = "SELECT schema_name FROM information_schema.schemata";
				MySqlCommand cmd = new MySqlCommand(query, conn);
				rdr = cmd.ExecuteReader();
				Console.WriteLine("<form name='form1' id='form1' method='post' action='mysql-cgi.exe'>");
				Console.WriteLine("<select id='schema' name='schema' size='3' " +
						"style='font-family: Courier New, Courier, Arial; font-size: 16px; background-color: wheat;'>");

				while (rdr.Read()) 
				{
					count += 1;
					schema = rdr.GetString(0);
					Console.WriteLine("<option value='" + schema + "'>" + schema + "</option>");
				} // WHILE over schemas
				Console.WriteLine("</select>");
				Console.WriteLine("<BR><BR><input type='submit' style='font-size: 16px;'" +
							"value='Process the selected schema'>");

				Console.WriteLine("</form>");
			} catch (MySqlException ex) 
			{
				Console.WriteLine("<H2>Error: {0}" + ex.ToString() + "</H2>");
				Console.WriteLine("</body></html>");
				Environment.Exit(0);
			} finally 
			{
				if (rdr != null) 
				{
					rdr.Close();
				}

				if (conn != null) 
				{
					conn.Close();
				}
			} // finally
			
			return;
		} // end of generate_choose_schema

		static public void generate_choose_table()
		{
			string schema = data_fields["schema"];
			string table_name;
			string numbered_table;
			int count;
			string cs = @"server=localhost;userid=root;
				password=mypassword;database=" + schema;

			Console.WriteLine("<H3>Choose one of the tables under the '" + schema + "' schema</H3>");

			MySqlConnection conn = null;
			MySqlDataReader rdr = null;
			count = 0;
			try 
			{
				string query = "SELECT table_name from information_schema.tables where table_schema = '" + schema + "'";
				conn = new MySqlConnection(cs);
				conn.Open();

				MySqlCommand cmd = new MySqlCommand(query, conn);
				rdr = cmd.ExecuteReader();
				Console.WriteLine("<form name='form1' id='form1' method='post' action='mysql-cgi.exe'>");
				Console.WriteLine("<input type='hidden' name='schema' id='schema' value='" + schema + "'>");
				Console.WriteLine("<select id='table' name='table' size='3' " +
						"style='font-family: Courier New, Courier, Arial; font-size: 16px; background-color: wheat;'>");

				while (rdr.Read()) 
				{
					count += 1;
					table_name = rdr.GetString(0);
					numbered_table = "(" + count.ToString() + ") " + table_name;
					Console.WriteLine("<option value='" + table_name + "'>" + numbered_table + "</option>");
				} // WHILE over schemas
				Console.WriteLine("</select>");
				Console.WriteLine("<BR><BR><input type='submit' style='font-size: 16px;'" +
							"value='Process the selected table_name'>");
				Console.WriteLine("</form>");
				Console.WriteLine("<BR><BR>Found " + count + " tables under " + schema + "<BR>");
			} catch (MySqlException ex) 
			{
				Console.WriteLine("<H2>Error: {0}" + ex.ToString() + "</H2>");
				Console.WriteLine("</body></html>");
				Environment.Exit(0);
			} finally 
			{
				if (rdr != null) 
				{
					rdr.Close();
				}

				if (conn != null) 
				{
					conn.Close();
				}
			} // finally
			Console.WriteLine("<BR><BR>" + top_level_href);
			return;

		} // end of generate_choose_table

		static void describe_table()
		{
			string schema = data_fields["schema"];
			string table = data_fields["table"];

			Console.WriteLine("<H3>Describe table '" + table + "' under schema '" + schema + "'</H3>");

			int count;
			string cs = "server=localhost;userid=root;password=mypassword;database=" + schema;

			MySqlConnection conn = null;
			MySqlDataReader rdr = null;
			count = 0;
			try 
			{
				conn = new MySqlConnection(cs);
				conn.Open();
        

				string query = "SELECT column_name colname,ordinal_position ord,is_nullable is_null," +
						"ifnull(column_comment,'--') comment,ifnull(CHARACTER_MAXIMUM_LENGTH,'--') c_maxlen," +
						"COLUMN_TYPE col_type,COLUMN_KEY col_key,ifnull(EXTRA,'--') extra" +
						" FROM information_schema.columns WHERE table_name = '" + table +
						"' AND table_schema = '" + schema + "'";

				debug_print("<H3>SQL is<BR>" + query +"</H3>");
				MySqlCommand cmd = new MySqlCommand(query, conn);
				rdr = cmd.ExecuteReader();

				while (rdr.Read()) 
				{
					count += 1;
					if ( count == 1 ){
						Console.WriteLine("<TABLE border='1' cellspacing='0' cellpadding='3'>");
						Console.WriteLine("<THEAD>");
						Console.WriteLine("<TR style='background: gainsboro;'><TH>Column</TH><TH>Data Type</TH><TH>Maxlen</TH>");
						Console.WriteLine("<TH>Nullable ?</TH><TH>Key</TH><TH>Extra</TH><TH>Comment</TH>");
						Console.WriteLine("</THEAD>");
						Console.WriteLine("<TBODY>");
					}
					Console.Write("<TR>");
					Console.Write("<TD>" + rdr.GetString(0) + "</TD>");
					Console.Write("<TD>" + rdr.GetString(5) + "</TD>");
					Console.Write("<TD>" + rdr.GetString(4) + "</TD>");
					Console.Write("<TD>" + rdr.GetString(2) + "</TD>");
					Console.Write("<TD>" + rdr.GetString(6) + "</TD>");
					Console.Write("<TD>" + rdr.GetString(7) + "</TD>");
					Console.Write("<TD>" + rdr.GetString(3) + "</TD>");
					Console.Write("</TR>");
				} // WHILE over schemas
				if ( count == 0 ) {
					Console.WriteLine("No columns found for table '" + table + "' under schema '" + schema + "'");
				}
				else {
					Console.WriteLine("</TBODY></TABLE>");
				}

			} catch (MySqlException ex) 
			{
				Console.WriteLine("<H2>Error:" + ex.ToString() + "</H2>");
				Console.WriteLine("</body></html>");
				Environment.Exit(0);
			} finally 
			{
				if (rdr != null) 
				{
					rdr.Close();
				}

				if (conn != null) 
				{
					conn.Close();
				}
			} // finally

			Console.WriteLine("<BR><BR>" + top_level_href);
			Console.WriteLine("<BR><BR><A HREF='mysql-cgi.exe?schema=" + schema + "'>Back To Table Selection for " + schema + "</A>");
			return;
		}

		[STAThread] // uses single threaded apartment model (STA)
		static void Main(string[] args) {
		Console.WriteLine("Content-Type: text/html\n\n");
		Console.WriteLine("<html><head><title>" + title_string + "</title></head>");
		Console.WriteLine("<body><h2>" + title_string + "</h2>");
		debug_print("The Common Gateway Interface version (env: GATEWAY_INTERFACE): " + System.Environment.GetEnvironmentVariable("GATEWAY_INTERFACE"));

		debug_print("<br/>The name and version of the protocol (env SERVER_PROTOCOL): " + System.Environment.GetEnvironmentVariable("SERVER_PROTOCOL"));
		debug_print("<br/>The request method used (env: REQUEST_METHOD): " + System.Environment.GetEnvironmentVariable("REQUEST_METHOD"));
		debug_print("<br/>Extra path information passed to the CGI program (env: PATH_INFO): " + System.Environment.GetEnvironmentVariable("PATH_INFO"));
		debug_print("<br/>The translated version of the path (env: PATH_TRANSLATED): " + System.Environment.GetEnvironmentVariable("PATH_TRANSLATED"));

		// string input_data = "";
		if (System.Environment.GetEnvironmentVariable("REQUEST_METHOD").Equals("POST")) {
			DataLength = Convert.ToInt32(System.Environment.GetEnvironmentVariable("CONTENT_LENGTH"));
			debug_print("DataLength = " + DataLength);
			if (DataLength > max_post_length) DataLength = max_post_length;  // Max length for POST data
				for (int i = 0; i < DataLength; i++)
					input_data += Convert.ToChar(Console.Read()).ToString();
				// debug_print("<br/>Post Data length: " + DataLength.ToString() + " Post data: " + input_data);
		}
		else {
			input_data = System.Environment.GetEnvironmentVariable("QUERY_STRING");
			if ( input_data == null ) {
				DataLength = 0;
				input_data = "";
				debug_print("<br/>The GET Query String was not specified");
			}
			else {
				DataLength = input_data.Length;
				debug_print("<br/>The GET Query String (env: QUERY_STRING): " + input_data);
			}
		}
		parse_input_fields();
		debug_print("<H3>num_vars = " + num_vars + "</H3>");

		try
		{
			if ( num_vars == 0 ){
				debug_print("<H3>No variables</H3>");
				generate_choose_schema();
			}
			else {
				if ( data_fields.ContainsKey("table") ) {
					debug_print("Detected table " + data_fields["table"]);
					describe_table();
				}
				else {
					if ( data_fields.ContainsKey("schema") ) {
						debug_print("Detected schema " + data_fields["schema"]);
						generate_choose_table();
					}
					else {
						Console.WriteLine("<H3>We have funky parameters</H3>");
					}
				}
			} // ELSE
		} catch ( Exception ex )
		{
			Console.WriteLine(ex.Message);
		} finally
		{
		} // finally


		Console.WriteLine("</body></html>");
		} // end of Main
	} // end of class ConsoleApplication1
} // end of namespace ConsoleApplication1
