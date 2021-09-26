﻿using ILRuntime.Runtime.Stack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILRuntime.Runtime.Intepreter
{
    public class ILRuntimeException : Exception
    {
        string stackTrace;
        string thisInfo, localInfo;
        internal ILRuntimeException(string message, ILIntepreter intepreter, CLR.Method.ILMethod method, Exception innerException = null)
            : base(message, innerException)

        {
            Debugger.DebugService ds = intepreter.AppDomain.DebugService;
            if (innerException is ILRuntimeException)
            {
                ILRuntimeException e = innerException as ILRuntimeException;
                stackTrace = e.stackTrace;
                thisInfo = e.thisInfo;
                localInfo = e.localInfo;
            }
            else
            {
                stackTrace = ds.GetStackTrace(intepreter);
                if (method.HasThis)
                    thisInfo = ds.GetThisInfo(intepreter);
                else
                    thisInfo = "";
                localInfo = ds.GetLocalVariableInfo(intepreter);
            }
        }

        public override string StackTrace
        {
            get
            {
                return stackTrace;
            }
        }

        public string ThisInfo
        {
            get { return thisInfo; }
        }

        public string LocalInfo
        {
            get
            {
                return localInfo;
            }
        }

        public override string ToString()
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine(Message);
            if (!string.IsNullOrEmpty(ThisInfo))
            {
                message.AppendLine("this:");
                message.AppendLine(ThisInfo);
            }
            message.AppendLine("Local Variables:");
            message.AppendLine(LocalInfo);
            message.AppendLine(StackTrace);
            if (InnerException != null)
                message.AppendLine(InnerException.ToString());
            return message.ToString();
        }
    }
}
