using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using IContainer = Autofac.IContainer;

namespace WPF_CaliburnMicro
{
   public class Bootstrapper : BootstrapperBase
   {
      readonly List<Assembly> assemblies =
         new List<Assembly>()
         {
            Assembly.GetExecutingAssembly(),
            Assembly.Load("WPF_CaliburnMicro.Views"),
            Assembly.Load("WPF_CaliburnMicro.ViewModels")
         };

      private static IContainer Container;

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

      protected override void Configure()
      {
         var builder = new ContainerBuilder();

         builder.RegisterType<WindowManager>()
             .AsImplementedInterfaces()
             .SingleInstance();

         builder.RegisterType<EventAggregator>()
             .AsImplementedInterfaces()
             .SingleInstance();

         // Register all the view models
         builder.RegisterAssemblyTypes(SelectAssemblies().ToArray())
            .Where(type => type.Name.EndsWith("ViewModel"))
            .Where(type => type.GetInterface(typeof(INotifyPropertyChanged).Name) != null)
            .AsSelf()
            .InstancePerDependency();

         // Register all the views
         builder.RegisterAssemblyTypes(SelectAssemblies().ToArray())
            .Where(type => type.Name.EndsWith("View"))
            .AsSelf()
            .InstancePerDependency();

         Container = builder.Build();
      }

      protected override IEnumerable<object> GetAllInstances(Type service)
      {
         var type = typeof(IEnumerable<>).MakeGenericType(service);
         return Container.Resolve(type) as IEnumerable<object>;
      }

      protected override IEnumerable<Assembly> SelectAssemblies() => assemblies;

      protected override object GetInstance(Type service, string key)
      {
         if (string.IsNullOrWhiteSpace(key))
         {
            if (Container.IsRegistered(service))
               return Container.Resolve(service);
         }
         else
         {
            if (Container.IsRegisteredWithKey(key, service))
               return Container.ResolveKeyed(key, service);
         }

         var msgFormat = "Could not locate any instances of contract {0}.";
         var msg = string.Format(msgFormat, key ?? service.Name);
         throw new Exception(msg);
      }

      // Not needed for Autofac container
      protected virtual void ConfigureContainer(ContainerBuilder builder) { }

      protected override void BuildUp(object instance)
      {
         Container.InjectProperties(instance);
      }

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
