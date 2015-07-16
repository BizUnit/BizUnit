//---------------------------------------------------------------------
// File: Pop3ConnectException.cs
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
	using System;

	internal class Pop3ConnectException : Exception
	{
		private readonly string _exceptionString;

		internal Pop3ConnectException()
		{
			_exceptionString = null;
		}

		internal Pop3ConnectException(string exceptionString)
		{
			_exceptionString = exceptionString;
		}

		internal Pop3ConnectException(string exceptionString, Exception ex) : base(exceptionString,ex)
		{
		}

		public override string ToString()
		{
			return _exceptionString;
		}
	}
}
