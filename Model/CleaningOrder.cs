

using System;
using System.Text.Json.Serialization;

namespace Opgave2_ServiceBestiller.Model
{
    /// <summary>
    /// Class <c>CleaningOrder</c> represents a service order for cleaning services.
    /// </summary>
    public class CleaningOrder : ServiceOrder

    {
        private int areaM2;

        // Area to be cleaned in square meters
        public int AreaM2
        {
            get => areaM2;
            set
            {
                // Validates that the area is greater than 0
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Arealet skal være større end 0 m².");
                areaM2 = value;
            }
        } 

        // Indicates whether window cleaning is included
        public bool IncludeWindows { get; set; }

        // Calculates the price based on area and window cleaning option
        protected override decimal CalculatePrice()
        { 
            return AreaM2 * AppConstants.PriceM2 + 
                (IncludeWindows ? AppConstants.PriceWindows : 0m);
        }
        public override string Summary()
        {
            // Extends the base summary with area information
            return base.Summary() + $" – Rengøring {AreaM2} m²";
        }
    }
}
