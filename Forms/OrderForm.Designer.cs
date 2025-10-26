using Opgave2_ServiceBestiller.Model;
using Opgave2_ServiceBestiller.Repo;

namespace Opgave2_ServiceBestiller.Forms
{
    partial class OrderForm : Form
    {
        private readonly ComboBox cmbType = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly TextBox txtCustomer = new();
        private readonly TextBox txtPhone = new();
        private readonly TextBox txtAddress = new();
        private readonly ComboBox cmbStatus = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly Panel pnlDynamic = new() { Dock = DockStyle.Top, Height = 120 };
        private readonly Label lblPrice = new() { Dock = DockStyle.Bottom, Height = 24, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
        private readonly Button btnOk = new() { Text = "OK", DialogResult = DialogResult.OK };
        private readonly Button btnCancel = new() { Text = "Annullér", DialogResult = DialogResult.Cancel };

        // Dynamic controls
        private NumericUpDown numArea = new() { Minimum = 1, Maximum = 10000, Value = 50 };
        private CheckBox chkWindows = new() { Text = "Vinduespuds" };

        private TextBox txtDevice = new();
        private CheckBox chkWarranty = new() { Text = "Under garanti" };

        private NumericUpDown numHours = new() { Minimum = 1, Maximum = 8, Value = 2 };
        private CheckBox chkOnsite = new() { Text = "Onsite" };

#nullable enable
        /// <summary>
        /// Gets the resulting ServiceOrder after the form is submitted.
        /// </summary>
        public ServiceOrder? Result { get; private set; }

        /// <summary>
        /// Initializes a new instance of the OrderForm class.
        /// If an existing ServiceOrder is provided, 
        /// the form is populated with its data for editing.
        /// </summary>
        public OrderForm(ServiceOrder? existing = null) : base()
        {
            Text = AppConstants.OrderFormTitle;
            Width = 520; Height = 420;

            // Center the form on the screen
            StartPosition = FormStartPosition.CenterScreen;

            // Fixed size dialog
            FormBorderStyle = FormBorderStyle.FixedDialog;

            // Disable maximize and minimize buttons
            MaximizeBox = false;
            MinimizeBox = false;

            // Set up layout. Use TableLayoutPanel for main layout 
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 8, Padding = new Padding(8) };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));

            // Add labels to layout
            layout.Controls.Add(new Label { Text = "Type" }, 0, 0);
            layout.Controls.Add(cmbType, 1, 0);
            layout.Controls.Add(new Label { Text = "Kunde" }, 0, 1);
            layout.Controls.Add(txtCustomer, 1, 1);
            layout.Controls.Add(new Label { Text = "Telefon" }, 0, 2);
            layout.Controls.Add(txtPhone, 1, 2);
            layout.Controls.Add(new Label { Text = "Adresse" }, 0, 3);
            layout.Controls.Add(txtAddress, 1, 3);
            layout.Controls.Add(new Label { Text = "Status" }, 0, 4);
            layout.Controls.Add(cmbStatus, 1, 4);
            layout.Controls.Add(new Label { Text = "Specifikt" }, 0, 5);
            layout.Controls.Add(pnlDynamic, 1, 5);

            var buttons = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Bottom, Height = 40 };
            buttons.Controls.AddRange(new Control[] { btnOk, btnCancel });

            Controls.Add(layout);
            Controls.Add(lblPrice);
            Controls.Add(buttons);

            // Initialize drop down menu and event handlers
            cmbType.Items.AddRange(new object[] { AppConstants.Cleaning, AppConstants.Repair, AppConstants.ITSupport });
            cmbType.SelectedIndex = 0;

            // Redisplay panel when selected item changes
            cmbType.SelectedIndexChanged += (_, __) => BuildDynamic();

            // Update status field
            cmbStatus.DataSource = Enum.GetValues(typeof(OrderStatus));

            // Button event handlers for OK and Cancel buttons
            btnOk.Click += (_, __) => Save();
            btnCancel.Click += (_, __) => { DialogResult = DialogResult.Cancel; };

            // If editing an existing order, load its data
            if (existing != null)
            {
                LoadExisting(existing);
            }
            else
            {
                // New order, build dynamic panel with empty fields
                BuildDynamic();
                UpdatePrice();
            }
        }
#nullable disable

        /// <summary>
        /// Loads an existing ServiceOrder into the form for editing.
        /// </summary>
        private void LoadExisting(ServiceOrder serviceOrder)
        {
            txtCustomer.Text = serviceOrder.CustomerName;
            txtPhone.Text = serviceOrder.Phone;
            txtAddress.Text = serviceOrder.Address;
            cmbStatus.SelectedItem = serviceOrder.Status;

            // Set the Type combo box text based on the kind of service order
            cmbType.SelectedItem = serviceOrder.Kind switch
            {
                // Fixed strings to match those used in the combo box
                AppConstants.Cleaning => AppConstants.Cleaning,
                AppConstants.Repair => AppConstants.Repair,
                AppConstants.ITSupport => AppConstants.ITSupport,
                _ => "?"
            };

            BuildDynamic();

            switch (serviceOrder)
            {
                case CleaningOrder c:
                    numArea.Value = c.AreaM2;
                    chkWindows.Checked = c.IncludeWindows;
                    break;
                case RepairOrder r:
                    txtDevice.Text = r.DeviceType;
                    chkWarranty.Checked = r.UnderWarranty;
                    break;
                case ITSupportOrder i:
                    numHours.Value = i.Hours;
                    chkOnsite.Checked = i.Onsite;
                    break;
            }

            Result = serviceOrder;
            UpdatePrice();
        }

        private void BuildDynamic()
        {
            pnlDynamic.Controls.Clear();
            var holder = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };

            if (cmbType.SelectedItem?.ToString() == AppConstants.Cleaning)
            {
                holder.Controls.Add(new Label { Text = "m²:" });
                holder.Controls.Add(numArea);
                holder.Controls.Add(chkWindows);
            }
            else if (cmbType.SelectedItem?.ToString() == AppConstants.Repair)
            {
                holder.Controls.Add(new Label { Text = "Enhed:" });
                holder.Controls.Add(txtDevice);
                holder.Controls.Add(chkWarranty);
            }
            else
            {
                holder.Controls.Add(new Label { Text = "Timer:" });
                holder.Controls.Add(numHours);
                holder.Controls.Add(chkOnsite);
            }

            pnlDynamic.Controls.Add(holder);
        }

        private void Save()
        {
            try
            {
                // Retrive common ServiceOrder form fields
                var orderFormField = (
                    Kind : cmbType.SelectedItem?.ToString(),
                    CustomerName: txtCustomer.Text, 
                    Phone: txtPhone.Text, 
                    Address: txtAddress.Text, 
                    Status: (OrderStatus)cmbStatus.SelectedItem!
                );

                // Create derived ServiceOrder based on selected type/kind
                ServiceOrder order = orderFormField.Kind switch
                {
                    AppConstants.Cleaning  => 
                        new CleaningOrder { AreaM2 = (int)numArea.Value, IncludeWindows = chkWindows.Checked },

                    AppConstants.Repair => 
                        new RepairOrder { DeviceType = txtDevice.Text, UnderWarranty = chkWarranty.Checked },

                    AppConstants.ITSupport => 
                        new ITSupportOrder { Hours = (int)numHours.Value, Onsite = chkOnsite.Checked },

                    _ => throw new InvalidOperationException("Ukendt ordretype")
                };

                // Declare Kind as non-null ("!") after validation
                order.Kind = orderFormField.Kind!;

                // Transfer common fields to new order instance
                order.CustomerName = orderFormField.CustomerName;
                order.Phone = orderFormField.Phone;
                order.Address = orderFormField.Address;
                order.Status = orderFormField.Status;

                // If editing an existing order, update it; otherwise set Result to new order
                Result = Result is null ? order : UpdateExisting(Result, order);

                // Update price label
                UpdatePrice();

                // Close dialog with OK result
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Valideringsfejl");
            }
        }

        /// <summary>
        /// Updates an existing ServiceOrder instance with data from a source instance. 
        /// </summary>
        private ServiceOrder UpdateExisting(ServiceOrder target, ServiceOrder source)
        {
            // Transfer common fields
            target.Kind = source.Kind;
            target.CustomerName = source.CustomerName;
            target.Phone = source.Phone;
            target.Address = source.Address;
            target.Status = source.Status;

            // Transfer specific fields based on derived type
            switch (target, source)
            {
                case (CleaningOrder t, CleaningOrder s): 
                    t.AreaM2 = s.AreaM2; 
                    t.IncludeWindows = s.IncludeWindows; 
                    break;

                case (RepairOrder t, RepairOrder s): 
                    t.DeviceType = s.DeviceType; 
                    t.UnderWarranty = s.UnderWarranty; 
                    break;

                case (ITSupportOrder t, ITSupportOrder s): 
                    t.Hours = s.Hours; 
                    t.Onsite = s.Onsite; 
                    break;

                default:
                    // If types not matching, just return the source
                    return source;
            }
            return target;
        }

        /// <summary>
        /// Updates the price label based on current form inputs or existing result. 
        /// </summary>
        private void UpdatePrice()
        {
            // If no existing Order, create a preview order to estimate price
            if (Result == null)
            {
                ServiceOrder preview;
                switch (cmbType.SelectedItem?.ToString())
                {
                    case AppConstants.Cleaning:
                        preview = new CleaningOrder
                        {
                            AreaM2 = (int)numArea.Value,
                            IncludeWindows = chkWindows.Checked,
                            CustomerName = "",
                            Phone = "",
                            Address = ""
                        };
                        break;
                    case AppConstants.Repair:
                        preview = new RepairOrder
                        {
                            DeviceType = txtDevice.Text,
                            UnderWarranty = chkWarranty.Checked,
                            CustomerName = "",
                            Phone = "",
                            Address = ""
                        };
                        break;
                    default:
                        preview = new ITSupportOrder
                        {
                            Hours = (int)numHours.Value,
                            Onsite = chkOnsite.Checked,
                            CustomerName = "",
                            Phone = "",
                            Address = ""
                        };
                        break;
                }
                lblPrice.Text = $"Pris (estimat): {preview.Price:C}";
            }
            else // Existing order, show its price
            {
                lblPrice.Text = $"Pris: {Result.Price:C}";
            }
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
            // OrderForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(296, 197);
            Name = "OrderForm";
            Text = "OrderForm";
            ResumeLayout(false);
        }

        #endregion
    }
}