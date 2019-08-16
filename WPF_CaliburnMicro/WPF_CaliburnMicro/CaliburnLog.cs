using System;
using Caliburn.Micro;
using Serilog;

namespace WPF_CaliburnMicro
{
   /// <summary>
   /// Adapts Caliburn Micro's log to Serilog
   /// </summary>
   internal class CaliburnSerilogLog : ILog
   {
      private ILogger context;

      public CaliburnSerilogLog(Type t)
      {
         context = Log.ForContext(t);
      }

      public void Error(Exception exception)
      {
         context.Error(exception, "");
      }

      public void Info(string format, params object[] args)
      {
         // The Info level in Caliburn Micro includes a
         // lot of information that might be considered
         // more Trace information rather than info.
         context.Verbose(format, args);
      }

      public void Warn(string format, params object[] args)
      {
         context.Warning(format, args);
      }
   }
}
