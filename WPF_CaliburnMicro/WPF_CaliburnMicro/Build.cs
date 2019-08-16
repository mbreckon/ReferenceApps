namespace WPF_CaliburnMicro
{
   /// <summary>
   /// Provides information about the current
   /// build of the software.
   /// </summary>
   internal static class Build
   {
      public static bool IsDebug
      {
         get
         {
#if DEBUG
            return true;
#else
            return false;
#endif
         }
      }
   }
}
