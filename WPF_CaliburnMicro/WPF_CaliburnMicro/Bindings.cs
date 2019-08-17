using System;
using System.Diagnostics;

namespace WPF_CaliburnMicro
{
   /// <summary>
   /// Manages error handling for WPF data binding
   /// trace information. Registers itself as a
   /// listener and forwards messages to a given
   /// delegate.
   /// </summary>
   public class Bindings : TraceListener
   {
      private Action<string> onErrorAction;
      private string currentMessage;

      public Bindings()
      {
         // For some reason if you miss this line out
         // the registration below doesn't work
         PresentationTraceSources.Refresh();

         PresentationTraceSources
            .DataBindingSource
            .Listeners
            .Add(this);

         PresentationTraceSources
            .DataBindingSource
            .Switch.Level = SourceLevels.Error;
      }

      public void OnError(Action<string> onErrorAction) =>
         this.onErrorAction = onErrorAction;

      public override void Write(string message)
      {
         currentMessage += message;
      }

      public override void WriteLine(string message)
      {
         currentMessage += message;
         onErrorAction(currentMessage);
         currentMessage = "";
      }
   }
}
