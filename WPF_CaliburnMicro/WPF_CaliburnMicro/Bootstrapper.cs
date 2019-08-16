using System;
using System.Windows;
using Caliburn.Micro;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace WPF_CaliburnMicro
{
   public class Bootstrapper : BootstrapperBase
   {
      public Bootstrapper()
      {
         Initialize();
      }

      protected override void OnStartup(
         object sender,
         StartupEventArgs e)
      {
         if (Build.IsDebug)
            ConsoleWindow.Show();

         Log.Logger =
            new LoggerConfiguration()
               .WriteTo.Console(
                  theme: AnsiConsoleTheme.Code
               )
               .MinimumLevel.Is(
                  Build.IsDebug
                     ? LogEventLevel.Debug
                     : LogEventLevel.Information
               )
               .CreateLogger();

         Log.Information("Application starting...");
         DisplayRootViewFor<ShellViewModel>();
      }

      protected override void OnExit(object sender, EventArgs e)
      {
         Log.Information("Application exiting...");

         if (Build.IsDebug)
            ConsoleWindow.Close();

         base.OnExit(sender, e);
      }
   }
}
