
using System;
using System.Text.Json.Serialization;

namespace Opgave2_ServiceBestiller.Model
{
    /// <summary>
    /// Class <c>RepairOrder</c> represents a repair service order. 
    /// </summary>
    public class RepairOrder : ServiceOrder
    {
        // Type of device to be repaired
        public string DeviceType { get; set; } = ""; // f.eks. vaskemaskine

        // Indicates whether the device is still under warranty
        public bool UnderWarranty { get; set; }

        // Calculates the price of the repair order
        protected override decimal CalculatePrice()
        {
            // If the device is under warranty, the price is 0; otherwise, it's the diagnostic price
            return UnderWarranty ? 0m : AppConstants.DiagnosticPrice; // fast diagnose pris
        }

        public override string Summary()
        {
            // Returns a summary string including the device type
            return base.Summary() + $" – Reparation {DeviceType}";
        }
    }
}
