
using Opgave2_ServiceBestiller.Model;

namespace Opgave2_ServiceBestiller.Model
{
    /// <summary>
    /// Provides constant values related to pricing and configuration for service orders.
    /// Change these values to adjust pricing and application settings, and recomile the application.
    /// </summary>
    internal static class AppConstants
    {
        // Price per square meter for general services
        public const decimal PriceM2 = 15m;

        // Additional price for window cleaning services
        public const decimal PriceWindows = 200m;

        // Fixed price for diagnostic services
        public const decimal DiagnosticPrice = 499m;

        // Fixed price for onsite service visits
        public const decimal PriceOnsite = 900m;

        // Fixed price for remote service 
        public const decimal PriceRemote = 700m;

        // Name of the JSON file used for persisting service orders
        public const string ReposName = "ServiceOrders.json";

        // MainFormTitle of the ServiceBestiller application
        public const string MainFormTitle = "ServiceBestiller – Booking af håndværker";

        // OrderFormTitle of the ServiceBestiller application
        public const string OrderFormTitle = "Bestilling";

        // Service types
        // These should match the 'Kind' property values in ServiceOrder derived classes
        public const string Cleaning  = "Rengøring";
        public const string Repair    = "Reparation";
        public const string ITSupport = "IT-Support";
    }
}
