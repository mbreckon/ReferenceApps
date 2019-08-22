using Caliburn.Micro;

namespace WPF_CaliburnMicro
{
   public class ShellViewModel : PropertyChangedBase
   {
      public string Name
      {
         get { return name; }
         set
         {
            name = value;
            NotifyOfPropertyChange(() => Name);
            NotifyOfPropertyChange(() => CanSayHello);
         }
      }

      public bool CanSayHello
      {
         get { return !string.IsNullOrWhiteSpace(Name); }
      }

      public void SayHello()
      {
       //  MessageBox.Show(string.Format("Hello {0}!", Name)); //Don't do this in real life :)
      }

      private string name;
   }
}
