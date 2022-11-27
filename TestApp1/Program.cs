using System.Globalization;

namespace TestApp1
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Properties.Settings settings = Properties.Settings.Default;
            Thread.CurrentThread.CurrentCulture = new CultureInfo( settings.Culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( settings.Culture);
            Application.Run(new MDIParent1());
        }
    }
}