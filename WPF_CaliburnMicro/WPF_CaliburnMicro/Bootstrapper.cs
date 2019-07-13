using System.Windows;
using Caliburn.Micro;

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
         DisplayRootViewFor<ShellViewModel>();
      }
   }
}
