//---------------------------------------------------------------------
// File: ArgumentValidation.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2017, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;

namespace BizUnit.Common
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ArgumentValidation'
    public static class ArgumentValidation
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ArgumentValidation'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ArgumentValidation.CheckForNullReference<T>(T, string)'
        public static T CheckForNullReference<T>(T var, string varName)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ArgumentValidation.CheckForNullReference<T>(T, string)'
        {
            if (varName == null)
                throw new ArgumentNullException("varName");

            if (var == null)
                throw new ArgumentNullException(varName);

            return var;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ArgumentValidation.CheckForEmptyString(string, string)'
        public static string CheckForEmptyString(string variable, string variableName)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ArgumentValidation.CheckForEmptyString(string, string)'
        {
            CheckForNullReference(variable, variableName);

            if (variable.Length == 0)
                throw new ArgumentException("Expected non-empty string.", variableName);

            return variable;
        }
    }
}
