//---------------------------------------------------------------------
// File: ComplexTypeParameterFormatter.cs
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
using System.Collections.Generic;
using System.Web.UI;

namespace BizUnit.BizUnitOM
{
    [Obsolete("ComplexTypeParameterFormatter has been deprecated. Please investigate the use of BizUnit.Xaml.TestCase.")]
    public class ComplexTypeParameterFormatter : ITestStepParameterFormatter
    {
        public object[] FormatParameters(Type type, object[] args, Context ctx)
        {
            object[] retVal;

            if (typeof(IList<string>) == type)
            {
                IList<string> argsAsstringList = new List<string>();
                foreach (object arg in args)
                {
                    argsAsstringList.Add((string)ctx.ReadArgument(arg));
                }
                retVal = new object[1];
                retVal[0] = argsAsstringList;
            }
            else if (typeof(IList<Pair>) == type)
            {
                IList<Pair> argsAsPairList = new List<Pair>();
                for (int c = 0; c < args.Length; c += 2)
                {
                    argsAsPairList.Add(new Pair(ctx.ReadArgument(args[c]), ctx.ReadArgument(args[c + 1])));
                }
                retVal = new object[1];
                retVal[0] = argsAsPairList;
            }
            else
            {
                throw new ApplicationException(
                    string.Format("The type {0} is not supported in the BizBizUnit object model", type.FullName));
            }

            return retVal;
        }
    }
}
