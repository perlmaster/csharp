//////////////////////////////////////////////////////////////////////
//
// File      : mysql-list-schemas.cs
//
// Author    : Barry Kimelman
//
// Created   : November 2, 2019
//
// Purpose   : Test MySQL access by listing all of the available schemas
//
// Notes     : (none)
//
//////////////////////////////////////////////////////////////////////

using System;
using MySql.Data.MySqlClient; 

public class Example
{

    static void Main() 
    {
        string cs = @"server=localhost;userid=root;
            password=mypassword;database=myschema";

        MySqlConnection conn = null;
        MySqlDataReader rdr = null;

        try 
        {
            conn = new MySqlConnection(cs);
            conn.Open();
        
			string query = "SELECT schema_name FROM information_schema.schemata";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            rdr = cmd.ExecuteReader();

            while (rdr.Read()) 
            {
				Console.WriteLine(rdr.GetString(0));
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
