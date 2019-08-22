using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
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
         ConfigureLogging();
         ConfigureErrorHandling();

         Log.Information("Application starting...");

         DisplayRootViewFor<ShellViewModel>();
      }

      protected override IEnumerable<Assembly> SelectAssemblies() =>
         new List<Assembly>() {
            Assembly.GetExecutingAssembly(),
            Assembly.Load("WPF_CaliburnMicro.Views"),
            Assembly.Load("WPF_CaliburnMicro.ViewModels")
         };

      protected override void OnExit(object sender, EventArgs e)
      {
         Log.Information("Application exiting...");

         if (Build.IsDebug && !Debugger.IsAttached)
            ConsoleWindow.Close();

         base.OnExit(sender, e);
      }

      private void ConfigureLogging()
      {
         if (Build.IsDebug && !Debugger.IsAttached)
            ConsoleWindow.Show();

         // Serilog setup
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

         // WPF Data Binding Errors
         new Bindings()
            .OnError(x => Log.Error(x));

         // Caliburn Micro logs
         LogManager.GetLog = t => new CaliburnSerilogLog(t);
      }

      private void ConfigureErrorHandling()
      {
         AppDomain.CurrentDomain.UnhandledException +=
            (s, e) => new UnhandledError().Handle(e);

         TaskScheduler.UnobservedTaskException +=
            (s, e) => new UnhandledError().Handle(e);
      }
   }
}
