using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Opgave2_ServiceBestiller.Model;
using Opgave2_ServiceBestiller.Repo;

namespace Opgave2_ServiceBestiller.Forms
{
    /// <summary>
    /// Class <c>MainForm</c> is the main form of the application.
    /// It displays a list of service orders and provides buttons 
    /// to create, edit, delete, save, and load orders.
    /// It uses a DataGridView to show the orders and a Label to display the total revenue.
    /// It interacts with the in memory OrderRepository to manage the orders.
    /// It also persists orders to a file and loads them from a file.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly BindingList<ServiceOrder> orders = new();
        private readonly OrderRepository orderRepo = new();

        // Interactive buttons for managing service orders.
        // Each button has a Danish label, no need to parameterize in AppConstants.
        private readonly Button btnNew = new() { Text = "Ny" };
        private readonly Button btnEdit = new() { Text = "Rediger" };
        private readonly Button btnDelete = new() { Text = "Slet" };
        private readonly Button btnSave = new() { Text = "Gem" };
        private readonly Button btnLoad = new() { Text = "Hent" };

        // Represents a read-only DataGridView control that fills its container 
        // and does not automatically generate columns.
        private readonly DataGridView grid = new() 
        { 
            Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = false 
        };

        // Represents a label control that displays the total value.
        private readonly Label lblTotal = new() 
        { 
            Dock = DockStyle.Bottom, 
            Height = 24, 
            TextAlign = ContentAlignment.MiddleRight 
        };

        /// <summary>
        /// Constructor for MainForm.
        /// Initializes the form, sets up the UI components.
        /// </summary>
        public MainForm(string title) : base()
        {
            Text = title;

            // Center the form on the screen
            this.StartPosition = FormStartPosition.CenterScreen;

            // Set the size of the form
            Width = 900; Height = 600;

            // Layout panel for buttons at the top
            var panelTop = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40 };
            panelTop.Controls.AddRange(new Control[] { btnNew, btnEdit, btnDelete, btnSave, btnLoad });

            Controls.Add(grid);
            Controls.Add(panelTop);
            Controls.Add(lblTotal);

            // Set up the data grid columns and bind to the orders list
            grid.DataSource = orders;
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Id", DataPropertyName = "Id", Width = 50 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Type", DataPropertyName = "Kind", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Kunde", DataPropertyName = "CustomerName", Width = 200 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Status", DataPropertyName = "Status", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Pris", DataPropertyName = "Price", Width = 120, DefaultCellStyle = { Format = "C" } });
            // ^ Price column with currency formatting "C"

            // Event handlers for button clicks
            // Using discard parameters (_) as they are not needed
            btnNew.Click += (_, __) => CreateOrder();
            btnEdit.Click += (_, __) => EditSelected();
            btnDelete.Click += (_, __) => DeleteSelected();
            btnSave.Click += (_, __) => SaveSelected();
            btnLoad.Click += (_, __) => LoadSelected();

            // Update totals when the form is "->Shown"
            Shown += (_, __) => UpdateTotals();
        }

        // Using nullable directive to allow for nullability in Selected property
#nullable enable

        /// <summary>
        /// Gets the currently selected ServiceOrder from the grid.
        /// Invoked when editing or deleting an order.
        /// Returns null if no order is selected, indicated with "?".
        /// </summary>
        private ServiceOrder? Selected
        {
            get
            {
                // Use null-conditional operator to safely access DataBoundItem
                return grid.CurrentRow?.DataBoundItem as ServiceOrder;
            }
        }
#nullable disable

        /// <summary>
        /// Method <c>CreateOrder</c> opens the OrderForm to create a new service order.
        /// In case of successful creation, adds the new order to the repository and updates the UI.
        /// </summary>
        private void CreateOrder()
        {
            // Using 'using var' to ensure proper disposal of the dialog
            using var dlg = new OrderForm(null);

            // Show the dialog and check if the result is OK and a valid order is returned
            // Using pattern matching to ensure dlg.Result is not null
            if (dlg.ShowDialog(this) == DialogResult.OK && dlg.Result is { } order)
            {
                // Add the new order to the repository and the binding list for the grid/UI
                orderRepo.Add(order);
                orders.Add(order);

                // Update the totals display
                UpdateTotals();
            }
        }

        /// <summary>
        /// Opens an order form to edit the currently selected order.
        /// If the edit is confirmed, the grid is refreshed and the totals are updated.
        /// </summary>
        private void EditSelected()
        {
            // Return early if no order is selected
            if (Selected is null) return;

            // Open the order form with the selected order
            using var dlg = new OrderForm(Selected);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                // Refresh the grid to reflect any changes
                grid.Refresh();

                // Update the totals display
                UpdateTotals();
            }
        }

        /// <summary>
        /// Deletes the currently selected order after user confirmation.
        /// </summary>
        private void DeleteSelected()
        {
            // Return early if no order is selected
            if (Selected is null) return;

            if (MessageBox.Show(this, "Slet valgte bestilling?", "Bekræft", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // Remove the order from the repository and the binding list for the grid/UI
                orderRepo.Remove(Selected);
                orders.Remove(Selected);

                // Update the totals display
                UpdateTotals();
            }
        }

        /// <summary>
        /// Saves the current list of orders to a persisted file, prompting the user for confirmation if a file already
        /// exists.
        /// If there are no orders to save, a message is displayed to inform the user. If a
        /// persisted file already exists, the user is prompted to confirm whether to overwrite it.  
        /// In case of an error during the save operation, an error message is displayed.
        /// </summary>
        private void SaveSelected()
        {
            if (orders.Count == 0)
            {
                MessageBox.Show(this, "Ingen bestillinger at gemme.", "Ok");
                return;
            }

            // Prompt user for confirmation if a persisted file already exists
            bool doSave = true;
            if (orderRepo.PersistedFileExists())
            {
                doSave = MessageBox.Show(this, "Der findes allerede gemte bestillinger.\nOverskriv?", "Bekræft", MessageBoxButtons.YesNo) == DialogResult.Yes;
            }

            if (doSave)
            {
                try
                {
                    // Save the orders to the persisted file and show the filename to the user
                    string filename = orderRepo.SaveToPersistedFile();
                    MessageBox.Show($"Gemt i {filename}");
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Fejl: {e.Message}");
                }

            }
        }

        /// <summary>
        /// Loads the persisted orders into the in memory repo, optionally overwriting existing orders.
        /// If there are existing orders, the user is prompted to confirm whether to overwrite
        /// them with the persisted orders. The method clears the current orders and loads all persisted
        /// orders from the file. It refreshes the display grid and updates the total.
        /// </summary>
        private void LoadSelected()
        {
            // Prompt user for confirmation if there are existing orders in memory
            bool doLoad = true;
            if (!orderRepo.IsEmpty())
            {
                doLoad = MessageBox.Show(this, "Overskriv form med gemte bestillinger?", "Bekræft", MessageBoxButtons.YesNo) == DialogResult.Yes;
            }

            if (doLoad)
            {
                try
                {
                    // Clear current orders and load persisted orders from file
                    orders.Clear();
                    orderRepo.LoadPersistedFile();

                    // Add all loaded orders to the binding list for the grid/UI
                    foreach (var order in orderRepo.All())
                        orders.Add(order);

                    // Refresh the grid and update the totals display
                    grid.Refresh();
                    UpdateTotals();
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Fejl: {e.Message}");
                }
            }
        }

        /// <summary>
        /// Updates the total revenue display label with the sum of all order prices.
        /// </summary>
        private void UpdateTotals()
        {
            // Calculate the total price of all orders
            decimal totalPrice = 0;
            foreach (var order in orders)
            {
                totalPrice += order.Price;
            }

            // Update the label with formatted total price, including currency symbol
            lblTotal.Text = $"Total omsætning: {totalPrice:C}";
        }


        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(304, 182);
            Name = "MainForm";
            Text = "MainForm";
            ResumeLayout(false);
        }

        #endregion
    }
}