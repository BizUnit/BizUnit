//---------------------------------------------------------------------
// File: Pop3Statics.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2015, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

namespace BizUnit.CoreSteps.Utilities.Pop3
{
    using System.Text.RegularExpressions;

	/// <summary>
	/// Summary description for Pop3Statics.
	/// </summary>
	internal class Pop3Statics
	{
		internal const string DataFolder = @"c:\POP3Temp";

		internal static string FromQuotedPrintable(string inString)
		{
			string outputString;
			string inputString = inString.Replace("=\n","");

			if(inputString.Length > 3)
			{
				// initialise output string ...
				outputString = "";

				for(int x=0; x<inputString.Length;)
				{
					string s1 = inputString.Substring(x,1);

					if( (s1.Equals("=")) && ((x+2) < inputString.Length) )
					{
						string hexString = inputString.Substring(x+1,2);

						// if hexadecimal ...
						if( Regex.Match(hexString.ToUpper()
							,@"^[A-F|0-9]+[A-F|0-9]+$").Success )
						{
							// convert to string representation ...
							outputString += System.Text.Encoding.ASCII.GetString(new[] {System.Convert.ToByte(hexString,16)});
							x+= 3;
						}
						else
						{
							outputString += s1;
							++x;
						}
					}
					else
					{
						outputString += s1;
						++x;
					}
				}
			}
			else
			{
				outputString = inputString;
			}

			return outputString.Replace("\n","\r\n");
		}
	}
}
