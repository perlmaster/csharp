//////////////////////////////////////////////////////////////////////
//
// File      : Cgilib.cs
//
// Author    : Barry Kimelman
//
// Created   : November 23, 2019
//
// Purpose   : CGI library
//
// Notes     : (none)
//
//////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Cgilib
{
 
  public class Suite
  {
    public ArrayList items = new ArrayList();
		public string input_data = "";
		public int DataLength;
		public int max_post_length = 2048;
		public char[] SplitChars1 = new char[]{'&'} ;
		public char[] SplitChars2 = new char[]{'='} ;
		public int num_vars;
		public string[] input_fields;
		public Dictionary<string, string> data_fields = new Dictionary<string, string>();
		public List<string> parm_names = new List<string>();

//////////////////////////////////////////////////////////////////////
//
// Function  : parse_input_fields
//
// Purpose   : Parse the parameters passed to this CGI script
//
// Inputs    : (none)
//
// Output    : (none)
//
// Returns   : nothing
//
// Example   : parse_input_fields();
//
// Notes     : (none)
//
//////////////////////////////////////////////////////////////////////

		public void parse_input_fields()
		{
			if ( System.Environment.GetEnvironmentVariable("REQUEST_METHOD").Equals("POST") ) {
				DataLength = Convert.ToInt32(System.Environment.GetEnvironmentVariable("CONTENT_LENGTH"));
				if (DataLength > max_post_length) DataLength = max_post_length;  // Max length for POST data
					for (int i = 0; i < DataLength; i++)
						input_data += Convert.ToChar(Console.Read()).ToString();
			}
			else {
				input_data = System.Environment.GetEnvironmentVariable("QUERY_STRING");
				if ( input_data == null ) {
					DataLength = 0;
					input_data = "";
				}
				else {
					DataLength = input_data.Length;
				}
			}

			if ( DataLength == 0 ) {
				num_vars = 0;
			}
			else {
				input_fields = input_data.Split(SplitChars1) ;
				num_vars = input_fields.Length;
				for ( int var_index = 0 ; var_index < num_vars ; ++var_index ) {
					string[] parts = input_fields[var_index].Split(SplitChars2) ;
					int num_parts = parts.Length;
					data_fields.Add(parts[0], parts[1]);
					parm_names.Add(parts[0]);
				}
			}

			return;
		} // end of parse_input_fields

  } // class Suite
} // ns