//---------------------------------------------------------------------
// File: ObjectCreator.cs
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
using System.Reflection;

namespace BizUnit
{
    public class ObjectCreator
    {
        static public object CreateStep(string typeName, string assemblyPath)
        {
            object comp = null;
            Type ty;

            if (!string.IsNullOrEmpty(assemblyPath))
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                ty = assembly.GetType(typeName, true, false);
            }
            else
            {
                ty = Type.GetType(typeName);
            }

            if (ty != null)
            {
                comp = Activator.CreateInstance(ty);
            }

            return comp;
        }

        static public Type GetType(string typeName, string assemblyPath)
        {
            Type t;

            if (!string.IsNullOrEmpty(assemblyPath))
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                t = assembly.GetType(typeName, true, false);
            }
            else
            {
                t = Type.GetType(typeName);
            }

            return t;
        }
    }
}
