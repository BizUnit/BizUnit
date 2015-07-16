//---------------------------------------------------------------------
// File: InteropUtils.cs
// 
// Summary: 
//
// Author: Kevin B. Smith (http://www.kevinsmith.co.uk)
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2010, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

namespace BizUnit
{
	using System.Runtime.InteropServices;

	/// <summary>
	/// COM Interop helper class		
	/// </summary>
	public class COMInteropUtils
	{
		/// <summary>
		/// Helper method to release a reference to a COM object
		/// </summary>
		/// <param name="obj">The COM object</param>

		static public void COMReleaseObject(object obj)
		{
			if ( null != obj )
			{
				if ( Marshal.IsComObject(obj) )
				{
					while( 0 < Marshal.ReleaseComObject( obj ) )
					{
						;
					}
				}
			}
		}
	}
}
