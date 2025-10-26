
using System;
using System.Text.Json.Serialization;

namespace Opgave2_ServiceBestiller.Model
{
    /// <summary>
    /// Class <c>ITSupportOrder</c> represents an IT support service order.
    /// </summary>
    public class ITSupportOrder : ServiceOrder
    {
        private int hours;

        // Number of hours for IT support (1 to 8)
        public int Hours
        {
            get => hours;
            set
            {
                if (value < 1 || value > 8)
                    throw new ArgumentOutOfRangeException("Antal timer skal være mellem 1 and 8.");
                hours = value;
            }
        }

        // Indicates whether the service is onsite or remote
        public bool Onsite { get; set; }

        protected override decimal CalculatePrice()
        {
            // Price is based on hours and whether the service is onsite or remote
            return Hours * (Onsite ? 
                AppConstants.PriceOnsite : 
                AppConstants.PriceRemote);
        }

        public override string Summary()
        {
            // Summary includes the number of hours for IT support
            return base.Summary() + $" – IT {Hours} t";
        }
    }
}
