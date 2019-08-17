using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Serilog;

namespace WPF_CaliburnMicro
{
   internal class UnhandledError
   {
      public void Handle(UnhandledExceptionEventArgs e)
      {
         if (e.ExceptionObject is Exception)
         {
            Log.Fatal(e.ExceptionObject as Exception, string.Empty);
         }
         else
         {
            // Can this ever happen?
            Log.Fatal("Unhandled, unknown exception type");
         }

         if (!Debugger.IsAttached)
         {
            MessageBox.Show(
               "An unexpected error has occurred.\n"
                  + " The application will now close.", 
               "Unexpected Error",
               MessageBoxButton.OK,
               MessageBoxImage.Error
            );
         }

         Application.Current.Shutdown();
      }

      public void Handle(UnobservedTaskExceptionEventArgs e)
      {
         // Current decision to simply log the
         // exception and continue...
         Log.Fatal(e.Exception, "");
      }
   }
}
