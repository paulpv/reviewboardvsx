using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ReviewBoardVsx.Package
{
    public class MyLog
    {
        private static String NormalizeMethodName(String methodName)
        {
            if (!methodName.EndsWith(")"))
            {
                methodName += "(...)";
            }
            if (!methodName.Equals("()") && !methodName.StartsWith("."))
            {
                methodName = "." + methodName;
            }
            return methodName;
        }

        public static void DebugEnter(Object o, String methodName)
        {
            Debug.WriteLine(string.Format("+{0}{1}", o.ToString(), NormalizeMethodName(methodName)));
        }

        public static void DebugLeave(Object o, String methodName)
        {
            Debug.WriteLine(string.Format("-{0}{1}", o.ToString(), NormalizeMethodName(methodName)));
        }
    }
}
