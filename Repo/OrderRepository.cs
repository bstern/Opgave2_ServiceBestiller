
using Opgave2_ServiceBestiller.Model;
using System;
using System.Text.Json;
using static System.Windows.Forms.Design.AxImporter;

namespace Opgave2_ServiceBestiller.Repo
{
    /// <summary>
    /// Class <c>OrderRepository</c> tracs, saves, and loads service orders.
    /// The orders are stored in a JSON file defined by <c>AppConstants.ReposName</c>.
    /// The file is created if it does not exist when saving.
    /// The file is read when LoadPersistedFile() is invoked, 
    /// and an exception is thrown if the file is not found.
    /// </summary>
    internal class OrderRepository
    {
        private readonly List<ServiceOrder> orders = new List<ServiceOrder>();
        private int nextId = 1;

        /// <summary>
        /// Method <c>All</c> returns all service orders in the repository.
        /// </summary>
        public IReadOnlyList<ServiceOrder> All()
        {
            return orders;
        }

        /// <summary>
        /// Method <c>Add</c> adds a new service order to the repository.
        /// </summary>
        public void Add(ServiceOrder order) 
        {
            // Assign a unique Id to the order and
            // increment the nextId counter for future orders
            order.Id = nextId++;
            orders.Add(order);
        }

        /// <summary>
        /// Method <c>IsEmpty</c> checks if the repository is empty.
        /// </summary>
        public bool IsEmpty()
        {
            return orders.Count == 0;
        }

        /// <summary>
        /// Method <c>PersistedFileExists</c> checks if the persisted file exists.
        /// </summary>
        public bool PersistedFileExists()
        {
            return File.Exists(AppConstants.ReposName);
        }

        /// <summary>
        /// Method <c>SaveToPersistedFile</c> saves the current orders to the persisted JSON file.
        /// Relies on JsonDerivedType attributes for polymorphic serialization, in ServiceOrder class.
        /// Returns the file path of the saved file.
        /// </summary>
        public string SaveToPersistedFile()
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize<List<ServiceOrder>>(orders, options);
            File.WriteAllText(AppConstants.ReposName, json);

            return Directory.GetCurrentDirectory() + "\\" + AppConstants.ReposName;
        }

        /// <summary>
        /// Method <c>LoadPersistedFile</c> loads orders from the persisted JSON file.
        /// Replaces any existing orders in the in memory repository.
        /// Relies on JsonDerivedType attributes for polymorphic deserialization, in ServiceOrder class.
        /// Throws a FileNotFoundException if the file does not exist.
        /// </summary>
        public void LoadPersistedFile()
        {
            if (File.Exists(AppConstants.ReposName))
            {
                string json = File.ReadAllText(AppConstants.ReposName);
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                var loadedOrders = JsonSerializer.Deserialize<List<ServiceOrder>>(json, options);
                if (loadedOrders != null)
                {
                    orders.Clear();
                    orders.AddRange(loadedOrders);
                    if (orders.Count > 0)
                    {
                        nextId = orders.Max(o => o.Id.GetValueOrDefault()) + 1;
                    }
                }
            }
            else
                throw new FileNotFoundException($"{AppConstants.ReposName} ikke fundet.");
        }

        /// <summary>
        /// Method <c>Remove</c> removes a service order from the in memory repository.
        /// Throws exceptions if the order is null, has no Id, or does not exist in the repository.
        /// </summary>
        internal void Remove(ServiceOrder selected)
        {
            if (selected == null)
                throw new ArgumentNullException(nameof(selected), "Bestillingen kan ikke være null.");  

            if (selected.Id == null)
                throw new InvalidOperationException("Kan ikke slette en bestilling uden Id.");

            if (!orders.Contains(selected))
                throw new InvalidOperationException("Bestillingen findes ikke i repositoriet.");

            orders.Remove(selected);
        }
    }
}
