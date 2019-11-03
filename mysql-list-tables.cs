//////////////////////////////////////////////////////////////////////
//
// File      : mysql-list-tables.cs
//
// Author    : Barry Kimelman
//
// Created   : November 2, 2019
//
// Purpose   : Test MySQL access by listing all tables under a schema
//
// Notes     : (none)
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient; 

public class Example
{

    static void Main(string[] args) 
    {
		int num_args = args.Length;
		if ( num_args < 1 ) {
			Console.WriteLine("Usage : mysql-list-tables.exe schema");
			Environment.Exit(0);
		}
        string cs = @"server=localhost;userid=root;
            password=mypassword;database=myschema";

        MySqlConnection conn = null;
        MySqlDataReader rdr = null;

        try 
        {
            conn = new MySqlConnection(cs);
            conn.Open();
        
			string query = "select table_schema,table_name,table_type,row_format,table_rows," +
							"create_time, " + "ifnull(update_time,'++') U_Time, " +
							"ifnull(check_time,'++') C_Time " +
							"from information_schema.tables where table_schema = '" +
							args[0] + "'";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            rdr = cmd.ExecuteReader();

			string header1 = "Table Name";
			string header1u = "==========";
			int longest_table_name = header1.Length;
			string header2 = "Table Type";
			string header2u = "==========";
			int longest_table_type = header2.Length;
			string header3 = "Create Time";
			string header3u = "===========";
			int len;
			string str;
			List<string> tables_list = new List<string>();
			List<string> types_list = new List<string>();
			List<string> created_list = new List<string>();

			int count = 0;
            while (rdr.Read()) 
            {
				count += 1;
				str = rdr.GetString(1);
				tables_list.Add(str);
				len = str.Length;
				if ( len > longest_table_name ) {
					longest_table_name = len;
				}

				str = rdr.GetString(2);
				types_list.Add(str);
				len = str.Length;
				if ( len > longest_table_type ) {
					longest_table_type = len;
				}

				str = rdr.GetString(5);
				created_list.Add(str);
            } // WHILE
			int padding_tabname = -longest_table_name;
			string format_tabname = "{0," + padding_tabname + "}";

			int padding_tabtype = -longest_table_type;
			string format_tabtype = "{0," + padding_tabtype + "}";

			string[] tablist = tables_list.ToArray();
			string[] typlist = types_list.ToArray();
			string[] crelist = created_list.ToArray();
			string tab = string.Format(format_tabname,header1);
			string typ = string.Format(format_tabtype,header2);
			string cre;
			Console.WriteLine(tab + " " + typ + " " + header3);
			tab = string.Format(format_tabname,header1u);
			typ = string.Format(format_tabtype,header2u);

			Console.WriteLine(tab + " " + typ + " " + header3u);

			for ( int index = 0 ; index < count ; ++index ){
				tab = string.Format(format_tabname,tablist[index]);
				typ = string.Format(format_tabtype,typlist[index]);
				cre = crelist[index];
				Console.WriteLine(tab + " " + typ + " " + cre);
			}

        } catch (MySqlException ex) 
        {
            Console.WriteLine("Error: {0}",  ex.ToString());

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

        }
    }
}
