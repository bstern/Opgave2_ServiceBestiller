
using System;
using System.Text.Json.Serialization;

namespace Opgave2_ServiceBestiller.Model
{
    // JSON polymorphic (de-)/serialization configuration for file persistence
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(derivedType: typeof(CleaningOrder),  typeDiscriminator: "cleaning")]
    [JsonDerivedType(derivedType: typeof(RepairOrder),    typeDiscriminator: "repair")]
    [JsonDerivedType(derivedType: typeof(ITSupportOrder), typeDiscriminator: "support")]

    /// <summary>
    /// Class <c>ServiceOrder</c> is an abstract base class representing a generic service order.
    /// It must be inherited by specific service order types like CleaningOrder, RepairOrder, and ITSupportOrder.
    /// It implements the IPriced interface to provide pricing functionality.
    /// </summary>
    public abstract class ServiceOrder : IPriced
    {
        private int? id;
        public int? Id
        {
            get
            {
                return id;
            }
            set
            {
                // Id can only be set once; subsequent attempts will throw an exception
                if (id != null)
                    throw new InvalidOperationException("Id er allerede initialiseret og kan ikke forandres.");
                id = value;
            }
        }
        public string Kind { get; set; } = "";
        public string CustomerName { get; set; } = ""; 
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public DateTime CreatedAt { get; } = DateTime.Now;
        public OrderStatus Status { get; set; } = OrderStatus.New;

        public decimal Price
        {
            get
            {
                // Property demonstrates polymorphic behavior by calling the abstract method
                // implemented in derived classes
                return CalculatePrice(); 
            }
        }

        // Abstract method to be implemented by derived classes for price calculation
        protected abstract decimal CalculatePrice();

        // Provides a summary string for the service order
        // Should be overridden by derived classes to include more details
        public virtual string Summary()
        {
            return $"#{Id} – {CustomerName} ({Status})";
        }
    }
}
