
using System;
using Opgave2_ServiceBestiller.Forms;
using Opgave2_ServiceBestiller.Model;

namespace Opgave2_ServiceBestiller
{
    /// <summary>
    /// Class <c>Program</c> is the main entry point for the application.
    /// It initializes the application configuration and runs the main form "MainForm".
    /// </summary>
    internal static class Program
    {
        //  The main entry point for the application.

        [STAThread] /// Single-Threaded Apartment attribute for COM interoperability
        static void Main()
        {
            /// Set the culture to Danish (Denmark) for localization purposes
            /// Vain attempt to set culture, may not work in all environments.
            /// Mainly trying to change language in MessageBox button texts
            /// Most reliable way is to change Windows OS language settings.
            System.Threading.Thread.CurrentThread.CurrentCulture =
                new System.Globalization.CultureInfo("da-DK");
            System.Threading.Thread.CurrentThread.CurrentUICulture =
                new System.Globalization.CultureInfo("da-DK");

            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm(AppConstants.MainFormTitle));
        }
    }
}