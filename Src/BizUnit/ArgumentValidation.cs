//---------------------------------------------------------------------
// File: ArgumentValidation.cs
// 
// Summary: 
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
    using System;

    public static class ArgumentValidation
    {
        public static T CheckForNullReference<T>(T var, string varName)
        {
            if (varName == null)
                throw new ArgumentNullException("varName");

            if (var == null)
                throw new ArgumentNullException(varName);

            return var;
        }

        public static string CheckForEmptyString(string variable, string variableName)
        {
            CheckForNullReference(variable, variableName);

            if (variable.Length == 0)
                throw new ArgumentException("Expected non-empty string.", variableName);

            return variable;
        }
    }
}
