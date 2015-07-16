//---------------------------------------------------------------------
// File: BizUnitParameterFormatterAttribute.cs
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

using System;
using BizUnit.Common;

namespace BizUnit.BizUnitOM
{
    /// <summary>
    /// The BizUnitParameterFormatterAttribute Attribute is used to decorate 
    /// test step properties to specify a custom formatter to use when setting 
    /// a specific property.
    /// </summary>
    /// 
    /// <remarks>
    /// The following example demonstrates how to use the BizUnitParameterFormatterAttribute:
    /// 
    /// <code escaped="true">
    /// 	public class DBExecuteNonQueryStep : ITestStepOM
    /// 	{
    /// 	    private SqlQuery sqlQuery;
    ///         ...
    /// 
    ///         // Instruct BizUnit to use the type: BizUnit.SqlQueryParamFormatter 
    ///         // to format the object array when setting this property. Note, 
    ///         // BizUnit.SqlQueryParamFormatter should implement ITestStepParameterFormatter
    ///         [BizUnitParameterFormatter("BizUnit.SqlQueryParamFormatter")]
    /// 	    public SqlQuery SQLQuery
    /// 	    {
    /// 	        set { sqlQuery = value; }
    ///             get { return sqlQuery; }
    /// 	    }
    /// 
    ///         ...
    ///     }
    ///
    ///	</code>
    ///	
    ///	</remarks>
    [Obsolete("BizUnitParameterFormatterAttribute has been deprecated. Please investigate the use of BizUnit.Xaml.TestCase.")]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class BizUnitParameterFormatterAttribute : Attribute
    {
        public BizUnitParameterFormatterAttribute()
        {
            TypeName = "BizUnit.BizUnitOM.DefaultTestStepParameterFormatter";
            AssemblyPath = null;
        }

        public BizUnitParameterFormatterAttribute(string typeName)
        {
            ArgumentValidation.CheckForEmptyString(typeName, "typeName");

            TypeName = typeName;
            AssemblyPath = null;
        }

        public BizUnitParameterFormatterAttribute(string typeName, string assemblyPath)
        {
            ArgumentValidation.CheckForEmptyString(typeName, "typeName");
            // assemblyPath - optional

            TypeName = typeName;
            AssemblyPath = assemblyPath;
        }

        public string TypeName { get; private set; }
        public string AssemblyPath { get; private set; }
    }
}
