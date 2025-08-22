using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using Stock_Room.Models;
using Stock_Room.UI;

namespace Stock_Room.Forms
{
    /// <summary>
    /// Form for adding and editing inventory items with modern UI
    /// </summary>
    public partial class InventoryItemForm : Form
    {
        private DatabaseHelper databaseHelper;
        private InventoryItem? currentItem;
        private bool isEditMode = false;
        
        // UI Controls
        private ModernPanel mainPanel;
        private Label titleLabel;
        
        // Basic Information Group
        private GroupBox basicInfoGroup;
        private ModernTextBox itemNameTextBox;
        private ModernTextBox descriptionTextBox;
        private ModernTextBox skuTextBox;
        private ModernTextBox barcodeTextBox;
        
        // Category and Supplier Group
        private GroupBox categorySupplierGroup;
        private ModernComboBox categoryComboBox;
        private ModernComboBox supplierComboBox;
        private ModernButton addCategoryButton;
        private ModernButton addSupplierButton;
        
        // Stock Information Group
        private GroupBox stockInfoGroup;
        private NumericUpDown quantityNumeric;
        private NumericUpDown minimumStockNumeric;
        private NumericUpDown reorderLevelNumeric;
        private ModernTextBox unitTextBox;
        
        // Pricing Group
        private GroupBox pricingGroup;
        private NumericUpDown costPriceNumeric;
        private NumericUpDown sellingPriceNumeric;
        private Label profitMarginLabel;
        
        // Additional Information Group
        private GroupBox additionalInfoGroup;
        private ModernComboBox statusComboBox;
        private ModernTextBox locationTextBox;
        private ModernTextBox notesTextBox;
        
        // Buttons
        private ModernButton saveButton;
        private ModernButton cancelButton;
        private ModernButton deleteButton;

        public InventoryItemForm() : this(null) { }

        public InventoryItemForm(InventoryItem? item)
        {
            databaseHelper = new DatabaseHelper();
            currentItem = item;
            isEditMode = item != null;
            
            InitializeComponent();
            LoadDataAsync();
            
            if (isEditMode)
            {
                PopulateFields();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = isEditMode ? "Edit Inventory Item" : "Add New Inventory Item";
            this.Size = new Size(800, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = ResourceManager.Theme.Background;
            this.Font = ResourceManager.Typography.BodyMedium;

            CreateMainPanel();
            CreateFormControls();
            SetupEventHandlers();

            this.Controls.Add(mainPanel);
            this.ResumeLayout();
        }

        private void CreateMainPanel()
        {
            mainPanel = new ModernPanel
            {
                Size = new Size(this.Width - 20, this.Height - 50),
                Location = new Point(10, 10),
                HasShadow = true
            };

            // Title
            titleLabel = new Label
            {
                Text = isEditMode ? "Edit Inventory Item" : "Add New Inventory Item",
                Font = ResourceManager.Typography.HeaderMedium,
                ForeColor = ResourceManager.Theme.TextPrimary,
                Size = new Size(760, 40),
                Location = new Point(20, 20),
                BackColor = Color.Transparent
            };

            mainPanel.Controls.Add(titleLabel);
        }

        private void CreateFormControls()
        {
            int yPos = 80;

            // Basic Information Group
            basicInfoGroup = CreateGroupBox("Basic Information", yPos, 120);
            CreateBasicInfoControls(basicInfoGroup);
            yPos += 140;

            // Category and Supplier Group
            categorySupplierGroup = CreateGroupBox("Category & Supplier", yPos, 80);
            CreateCategorySupplierControls(categorySupplierGroup);
            yPos += 100;

            // Stock Information Group
            stockInfoGroup = CreateGroupBox("Stock Information", yPos, 80);
            CreateStockInfoControls(stockInfoGroup);
            yPos += 100;

            // Pricing Group
            pricingGroup = CreateGroupBox("Pricing", yPos, 80);
            CreatePricingControls(pricingGroup);
            yPos += 100;

            // Additional Information Group
            additionalInfoGroup = CreateGroupBox("Additional Information", yPos, 100);
            CreateAdditionalInfoControls(additionalInfoGroup);
            yPos += 120;

            // Buttons
            CreateButtons(yPos);

            mainPanel.Controls.AddRange(new Control[] 
            { 
                basicInfoGroup, categorySupplierGroup, stockInfoGroup, 
                pricingGroup, additionalInfoGroup 
            });
        }

        private GroupBox CreateGroupBox(string title, int y, int height)
        {
            return new GroupBox
            {
                Text = title,
                Font = ResourceManager.Typography.HeaderSmall,
                ForeColor = ResourceManager.Theme.TextPrimary,
                Size = new Size(720, height),
                Location = new Point(20, y),
                BackColor = Color.Transparent
            };
        }

        private void CreateBasicInfoControls(GroupBox parent)
        {
            // Item Name
            CreateLabel(parent, "Item Name *:", 20, 30);
            itemNameTextBox = new ModernTextBox
            {
                Location = new Point(20, 50),
                Size = new Size(300, 35),
                Placeholder = "Enter item name..."
            };

            // Description
            CreateLabel(parent, "Description:", 350, 30);
            descriptionTextBox = new ModernTextBox
            {
                Location = new Point(350, 50),
                Size = new Size(300, 35),
                Placeholder = "Enter description..."
            };

            // SKU
            CreateLabel(parent, "SKU:", 20, 85);
            skuTextBox = new ModernTextBox
            {
                Location = new Point(20, 105),
                Size = new Size(150, 35),
                Placeholder = "Enter SKU..."
            };

            // Barcode
            CreateLabel(parent, "Barcode:", 200, 85);
            barcodeTextBox = new ModernTextBox
            {
                Location = new Point(200, 105),
                Size = new Size(180, 35),
                Placeholder = "Enter barcode..."
            };

            parent.Controls.AddRange(new Control[] 
            { 
                itemNameTextBox, descriptionTextBox, skuTextBox, barcodeTextBox 
            });
        }

        private void CreateCategorySupplierControls(GroupBox parent)
        {
            // Category
            CreateLabel(parent, "Category *:", 20, 30);
            categoryComboBox = new ModernComboBox
            {
                Location = new Point(20, 50),
                Size = new Size(250, 35)
            };

            addCategoryButton = new ModernButton
            {
                Text = "+",
                Size = new Size(35, 35),
                Location = new Point(280, 50),
                BackColor = ResourceManager.Theme.Success
            };

            // Supplier
            CreateLabel(parent, "Supplier *:", 350, 30);
            supplierComboBox = new ModernComboBox
            {
                Location = new Point(350, 50),
                Size = new Size(250, 35)
            };

            addSupplierButton = new ModernButton
            {
                Text = "+",
                Size = new Size(35, 35),
                Location = new Point(610, 50),
                BackColor = ResourceManager.Theme.Success
            };

            parent.Controls.AddRange(new Control[] 
            { 
                categoryComboBox, addCategoryButton, supplierComboBox, addSupplierButton 
            });
        }

        private void CreateStockInfoControls(GroupBox parent)
        {
            // Quantity
            CreateLabel(parent, "Quantity:", 20, 30);
            quantityNumeric = new NumericUpDown
            {
                Location = new Point(20, 50),
                Size = new Size(100, 35),
                Minimum = 0,
                Maximum = 999999,
                DecimalPlaces = 0
            };

            // Minimum Stock
            CreateLabel(parent, "Min Stock:", 150, 30);
            minimumStockNumeric = new NumericUpDown
            {
                Location = new Point(150, 50),
                Size = new Size(100, 35),
                Minimum = 0,
                Maximum = 999999,
                DecimalPlaces = 0
            };

            // Reorder Level
            CreateLabel(parent, "Reorder Level:", 280, 30);
            reorderLevelNumeric = new NumericUpDown
            {
                Location = new Point(280, 50),
                Size = new Size(100, 35),
                Minimum = 0,
                Maximum = 999999,
                DecimalPlaces = 0
            };

            // Unit
            CreateLabel(parent, "Unit:", 410, 30);
            unitTextBox = new ModernTextBox
            {
                Location = new Point(410, 50),
                Size = new Size(100, 35),
                Placeholder = "pcs"
            };

            parent.Controls.AddRange(new Control[] 
            { 
                quantityNumeric, minimumStockNumeric, reorderLevelNumeric, unitTextBox 
            });
        }

        private void CreatePricingControls(GroupBox parent)
        {
            // Cost Price
            CreateLabel(parent, "Cost Price ($):", 20, 30);
            costPriceNumeric = new NumericUpDown
            {
                Location = new Point(20, 50),
                Size = new Size(120, 35),
                Minimum = 0,
                Maximum = 999999,
                DecimalPlaces = 2
            };

            // Selling Price
            CreateLabel(parent, "Selling Price ($):", 170, 30);
            sellingPriceNumeric = new NumericUpDown
            {
                Location = new Point(170, 50),
                Size = new Size(120, 35),
                Minimum = 0,
                Maximum = 999999,
                DecimalPlaces = 2
            };

            // Profit Margin
            profitMarginLabel = new Label
            {
                Text = "Profit Margin: $0.00 (0%)",
                Font = ResourceManager.Typography.BodyMedium,
                ForeColor = ResourceManager.Theme.TextSecondary,
                Size = new Size(200, 25),
                Location = new Point(320, 55),
                BackColor = Color.Transparent
            };

            parent.Controls.AddRange(new Control[] 
            { 
                costPriceNumeric, sellingPriceNumeric, profitMarginLabel 
            });
        }

        private void CreateAdditionalInfoControls(GroupBox parent)
        {
            // Status
            CreateLabel(parent, "Status:", 20, 30);
            statusComboBox = new ModernComboBox
            {
                Location = new Point(20, 50),
                Size = new Size(150, 35)
            };
            statusComboBox.Items.AddRange(new[] { "Active", "Inactive", "Discontinued" });
            statusComboBox.SelectedIndex = 0;

            // Location
            CreateLabel(parent, "Location:", 200, 30);
            locationTextBox = new ModernTextBox
            {
                Location = new Point(200, 50),
                Size = new Size(200, 35),
                Placeholder = "Storage location..."
            };

            // Notes
            CreateLabel(parent, "Notes:", 20, 85);
            notesTextBox = new ModernTextBox
            {
                Location = new Point(20, 105),
                Size = new Size(620, 35),
                Placeholder = "Additional notes..."
            };

            parent.Controls.AddRange(new Control[] 
            { 
                statusComboBox, locationTextBox, notesTextBox 
            });
        }

        private void CreateButtons(int yPos)
        {
            saveButton = new ModernButton
            {
                Text = isEditMode ? "Update Item" : "Save Item",
                Size = new Size(120, 40),
                Location = new Point(450, yPos),
                BackColor = ResourceManager.Theme.Success,
                Image = ResourceManager.DashboardIcons.GetSaveIcon()
            };

            cancelButton = new ModernButton
            {
                Text = "Cancel",
                Size = new Size(100, 40),
                Location = new Point(580, yPos),
                BackColor = ResourceManager.Theme.TextMuted,
                Image = ResourceManager.DashboardIcons.GetCancelIcon()
            };

            if (isEditMode)
            {
                deleteButton = new ModernButton
                {
                    Text = "Delete",
                    Size = new Size(100, 40),
                    Location = new Point(20, yPos),
                    BackColor = ResourceManager.Theme.Danger,
                    Image = ResourceManager.DashboardIcons.GetDeleteIcon()
                };
                mainPanel.Controls.Add(deleteButton);
            }

            mainPanel.Controls.AddRange(new Control[] { saveButton, cancelButton });
        }

        private void CreateLabel(Control parent, string text, int x, int y)
        {
            var label = new Label
            {
                Text = text,
                Font = ResourceManager.Typography.BodyMedium,
                ForeColor = ResourceManager.Theme.TextPrimary,
                Size = new Size(200, 20),
                Location = new Point(x, y),
                BackColor = Color.Transparent
            };
            parent.Controls.Add(label);
        }

        private void SetupEventHandlers()
        {
            saveButton.Click += SaveButton_Click;
            cancelButton.Click += CancelButton_Click;
            
            if (deleteButton != null)
                deleteButton.Click += DeleteButton_Click;

            costPriceNumeric.ValueChanged += UpdateProfitMargin;
            sellingPriceNumeric.ValueChanged += UpdateProfitMargin;

            addCategoryButton.Click += AddCategoryButton_Click;
            addSupplierButton.Click += AddSupplierButton_Click;

            // Validation
            itemNameTextBox.TextChanged += ValidateForm;
            categoryComboBox.SelectedIndexChanged += ValidateForm;
            supplierComboBox.SelectedIndexChanged += ValidateForm;
        }

        private async void LoadDataAsync()
        {
            try
            {
                // Load categories
                var categories = await databaseHelper.GetCategoriesAsync();
                categoryComboBox.Items.Clear();
                foreach (var category in categories)
                {
                    categoryComboBox.Items.Add(new ComboBoxItem(category.CategoryName, category.CategoryID));
                }

                // Load suppliers
                var suppliers = await databaseHelper.GetSuppliersAsync();
                supplierComboBox.Items.Clear();
                foreach (var supplier in suppliers)
                {
                    supplierComboBox.Items.Add(new ComboBoxItem(supplier.SupplierName, supplier.SupplierID));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load data: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateFields()
        {
            if (currentItem == null) return;

            itemNameTextBox.SetValue(currentItem.ItemName);
            descriptionTextBox.SetValue(currentItem.Description);
            skuTextBox.SetValue(currentItem.SKU);
            barcodeTextBox.SetValue(currentItem.Barcode);

            // Select category and supplier
            SelectComboBoxItem(categoryComboBox, currentItem.CategoryID);
            SelectComboBoxItem(supplierComboBox, currentItem.SupplierID);

            quantityNumeric.Value = currentItem.Quantity;
            minimumStockNumeric.Value = currentItem.MinimumStock;
            reorderLevelNumeric.Value = currentItem.ReorderLevel;
            unitTextBox.SetValue(currentItem.Unit);

            costPriceNumeric.Value = currentItem.CostPrice;
            sellingPriceNumeric.Value = currentItem.SellingPrice;

            statusComboBox.Text = currentItem.Status;
            locationTextBox.SetValue(currentItem.Location);
            notesTextBox.SetValue(currentItem.Notes);

            UpdateProfitMargin(null, EventArgs.Empty);
        }

        private void SelectComboBoxItem(ComboBox comboBox, int value)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.Value == value)
                {
                    comboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                var item = CreateInventoryItemFromInput();

                if (isEditMode)
                {
                    item.ItemID = currentItem!.ItemID;
                    await databaseHelper.UpdateInventoryItemAsync(item);
                    MessageBox.Show("Item updated successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    await databaseHelper.CreateInventoryItemAsync(item);
                    MessageBox.Show("Item created successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save item: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private async void DeleteButton_Click(object sender, EventArgs e)
        {
            if (currentItem == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete '{currentItem.ItemName}'?\n\nThis action cannot be undone.", 
                "Confirm Delete", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    await databaseHelper.DeleteInventoryItemAsync(currentItem.ItemID);
                    MessageBox.Show("Item deleted successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete item: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateProfitMargin(object sender, EventArgs e)
        {
            var costPrice = costPriceNumeric.Value;
            var sellingPrice = sellingPriceNumeric.Value;
            var margin = sellingPrice - costPrice;
            var percentage = costPrice > 0 ? (double)(margin / costPrice * 100) : 0;

            profitMarginLabel.Text = $"Profit Margin: ${margin:F2} ({percentage:F1}%)";
            profitMarginLabel.ForeColor = margin >= 0 ? ResourceManager.Theme.Success : ResourceManager.Theme.Danger;
        }

        private void ValidateForm(object sender, EventArgs e)
        {
            saveButton.Enabled = ValidateInput(false);
        }

        private bool ValidateInput(bool showMessages = true)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(itemNameTextBox.GetValue()))
                errors.Add("Item name is required.");

            if (categoryComboBox.SelectedItem == null)
                errors.Add("Category is required.");

            if (supplierComboBox.SelectedItem == null)
                errors.Add("Supplier is required.");

            if (errors.Count > 0 && showMessages)
            {
                MessageBox.Show($"Please fix the following errors:\n\n• {string.Join("\n• ", errors)}", 
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return errors.Count == 0;
        }

        private InventoryItem CreateInventoryItemFromInput()
        {
            return new InventoryItem
            {
                ItemName = itemNameTextBox.GetValue(),
                Description = descriptionTextBox.GetValue(),
                SKU = skuTextBox.GetValue(),
                Barcode = barcodeTextBox.GetValue(),
                CategoryID = ((ComboBoxItem)categoryComboBox.SelectedItem).Value,
                SupplierID = ((ComboBoxItem)supplierComboBox.SelectedItem).Value,
                Quantity = (int)quantityNumeric.Value,
                MinimumStock = (int)minimumStockNumeric.Value,
                ReorderLevel = (int)reorderLevelNumeric.Value,
                Unit = string.IsNullOrWhiteSpace(unitTextBox.GetValue()) ? "pcs" : unitTextBox.GetValue(),
                CostPrice = costPriceNumeric.Value,
                SellingPrice = sellingPriceNumeric.Value,
                Status = statusComboBox.Text,
                Location = locationTextBox.GetValue(),
                Notes = notesTextBox.GetValue(),
                CreatedDate = isEditMode ? currentItem!.CreatedDate : DateTime.Now,
                LastUpdated = DateTime.Now
            };
        }

        private void AddCategoryButton_Click(object sender, EventArgs e)
        {
            // TODO: Implement add category dialog
            MessageBox.Show("Add Category dialog will be implemented.", "Add Category", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AddSupplierButton_Click(object sender, EventArgs e)
        {
            // TODO: Implement add supplier dialog
            MessageBox.Show("Add Supplier dialog will be implemented.", "Add Supplier", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                databaseHelper?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Helper class for ComboBox items with display text and value
    /// </summary>
    public class ComboBoxItem
    {
        public string Text { get; set; }
        public int Value { get; set; }

        public ComboBoxItem(string text, int value)
        {
            Text = text;
            Value = value;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}