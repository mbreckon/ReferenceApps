using System.Runtime.InteropServices;

namespace WPF_CaliburnMicro
{
   /// <summary>
   /// A console window that can be shown for
   /// non-console applications.
   /// </summary>
   internal static class ConsoleWindow
   {
      [DllImport("Kernel32")]
      private static extern void AllocConsole();

      [DllImport("Kernel32")]
      private static extern void FreeConsole();

      public static void Show() => AllocConsole();
      public static void Close() => FreeConsole();
   }
}
