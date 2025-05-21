using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace ClothingShopApp
{
    public partial class MainForm : Form
    {
        private ClothingShop shop = new ClothingShop();
        private List<Product> currentDisplayedProducts = new List<Product>();
        private string currentFilePath = string.Empty;

        public MainForm()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Set up form properties
            this.Text = "Clothing Shop Management";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(1000, 600);

            // Initialize all UI components
            InitializeMenuStrip();
            InitializeInputControls();
            InitializeProductGrid();
            InitializeSortControls();
        }

        private void InitializeMenuStrip()
        {
            MenuStrip menuStrip = new MenuStrip();
            
            // File menu
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("File");
            fileMenu.DropDownItems.Add("New", null, (s, e) => NewShop());
            fileMenu.DropDownItems.Add("Open...", null, (s, e) => OpenShop());
            fileMenu.DropDownItems.Add("Save", null, (s, e) => SaveShop());
            fileMenu.DropDownItems.Add("Save As...", null, (s, e) => SaveShopAs());
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add("Exit", null, (s, e) => this.Close());
            
            // Edit menu
            ToolStripMenuItem editMenu = new ToolStripMenuItem("Edit");
            editMenu.DropDownItems.Add("Add Product", null, (s, e) => AddProductFromForm());
            editMenu.DropDownItems.Add("Delete Selected", null, (s, e) => DeleteSelectedProduct());
            editMenu.DropDownItems.Add("Modify Selected", null, (s, e) => ModifySelectedProduct());
            
            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(editMenu);
            
            this.Controls.Add(menuStrip);
            this.MainMenuStrip = menuStrip;
        }

        private void InitializeInputControls()
        {
            Panel inputPanel = new Panel();
            inputPanel.Dock = DockStyle.Top;
            inputPanel.Height = 200;
            inputPanel.BorderStyle = BorderStyle.FixedSingle;
            
            // Create input controls for each product property
            int yPos = 10;
            int labelWidth = 100;
            int controlWidth = 150;
            int spacing = 30;
            
            // ID
            Label lblId = new Label { Text = "ID:", Left = 10, Top = yPos, Width = labelWidth };
            TextBox txtId = new TextBox { Name = "txtId", Left = labelWidth + 15, Top = yPos, Width = controlWidth };
            inputPanel.Controls.Add(lblId);
            inputPanel.Controls.Add(txtId);
            yPos += spacing;
            
            // Type
            Label lblType = new Label { Text = "Type:", Left = 10, Top = yPos, Width = labelWidth };
            ComboBox cmbType = new ComboBox { Name = "cmbType", Left = labelWidth + 15, Top = yPos, Width = controlWidth };
            cmbType.DataSource = Enum.GetValues(typeof(ClothingType));
            inputPanel.Controls.Add(lblType);
            inputPanel.Controls.Add(cmbType);
            yPos += spacing;
            
            // Cut
            Label lblCut = new Label { Text = "Cut:", Left = 10, Top = yPos, Width = labelWidth };
            ComboBox cmbCut = new ComboBox { Name = "cmbCut", Left = labelWidth + 15, Top = yPos, Width = controlWidth };
            cmbCut.DataSource = Enum.GetValues(typeof(Cut));
            inputPanel.Controls.Add(lblCut);
            inputPanel.Controls.Add(cmbCut);
            yPos += spacing;
            
            // Color
            Label lblColor = new Label { Text = "Color:", Left = 10, Top = yPos, Width = labelWidth };
            TextBox txtColor = new TextBox { Name = "txtColor", Left = labelWidth + 15, Top = yPos, Width = controlWidth };
            inputPanel.Controls.Add(lblColor);
            inputPanel.Controls.Add(txtColor);
            yPos += spacing;
            
            // Fabric
            Label lblFabric = new Label { Text = "Fabric:", Left = 10, Top = yPos, Width = labelWidth };
            ComboBox cmbFabric = new ComboBox { Name = "cmbFabric", Left = labelWidth + 15, Top = yPos, Width = controlWidth };
            cmbFabric.DataSource = Enum.GetValues(typeof(Fabric));
            inputPanel.Controls.Add(lblFabric);
            inputPanel.Controls.Add(cmbFabric);
            yPos += spacing;
            
            // Size
            Label lblSize = new Label { Text = "Size:", Left = 10, Top = yPos, Width = labelWidth };
            ComboBox cmbSize = new ComboBox { Name = "cmbSize", Left = labelWidth + 15, Top = yPos, Width = controlWidth };
            cmbSize.DataSource = Enum.GetValues(typeof(Size));
            inputPanel.Controls.Add(lblSize);
            inputPanel.Controls.Add(cmbSize);
            yPos += spacing;
            
            // Brand
            Label lblBrand = new Label { Text = "Brand:", Left = 10, Top = yPos, Width = labelWidth };
            TextBox txtBrand = new TextBox { Name = "txtBrand", Left = labelWidth + 15, Top = yPos, Width = controlWidth };
            inputPanel.Controls.Add(lblBrand);
            inputPanel.Controls.Add(txtBrand);
            yPos += spacing;
            
            // Base Price
            Label lblPrice = new Label { Text = "Base Price:", Left = 10, Top = yPos, Width = labelWidth };
            NumericUpDown numPrice = new NumericUpDown { Name = "numPrice", Left = labelWidth + 15, Top = yPos, Width = controlWidth, DecimalPlaces = 2, Minimum = 0, Maximum = 10000 };
            inputPanel.Controls.Add(lblPrice);
            inputPanel.Controls.Add(numPrice);
            yPos += spacing;
            
            // Add Product Button
            Button btnAdd = new Button { Text = "Add Product", Left = labelWidth + 15, Top = yPos, Width = controlWidth };
            btnAdd.Click += (s, e) => AddProductFromForm();
            inputPanel.Controls.Add(btnAdd);
            
            this.Controls.Add(inputPanel);
        }

        private void InitializeProductGrid()
        {
            DataGridView grid = new DataGridView();
            grid.Name = "productGrid";
            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            
            // Add columns
            grid.Columns.Add("Id", "ID");
            grid.Columns.Add("Type", "Type");
            grid.Columns.Add("Cut", "Cut");
            grid.Columns.Add("Color", "Color");
            grid.Columns.Add("Fabric", "Fabric");
            grid.Columns.Add("Size", "Size");
            grid.Columns.Add("Brand", "Brand");
            grid.Columns.Add("BasePrice", "Base Price");
            grid.Columns.Add("FinalPrice", "Final Price");
            
            this.Controls.Add(grid);
        }

        private void InitializeSortControls()
        {
            Panel sortPanel = new Panel();
            sortPanel.Dock = DockStyle.Bottom;
            sortPanel.Height = 50;
            sortPanel.BorderStyle = BorderStyle.FixedSingle;
            
            // Sort by label
            Label lblSortBy = new Label { Text = "Sort by:", Left = 10, Top = 15, Width = 50 };
            sortPanel.Controls.Add(lblSortBy);
            
            // Sort by combo
            ComboBox cmbSortBy = new ComboBox { Name = "cmbSortBy", Left = 70, Top = 10, Width = 100 };
            cmbSortBy.Items.AddRange(new string[] { "Type", "Color", "Price" });
            cmbSortBy.SelectedIndex = 0;
            sortPanel.Controls.Add(cmbSortBy);
            
            // Direction combo
            ComboBox cmbDirection = new ComboBox { Name = "cmbDirection", Left = 180, Top = 10, Width = 100 };
            cmbDirection.Items.AddRange(new string[] { "Ascending", "Descending" });
            cmbDirection.SelectedIndex = 0;
            sortPanel.Controls.Add(cmbDirection);
            
            // Sort button
            Button btnSort = new Button { Text = "Sort", Left = 290, Top = 10, Width = 75 };
            btnSort.Click += (s, e) => SortProducts();
            sortPanel.Controls.Add(btnSort);
            
            this.Controls.Add(sortPanel);
        }

        private void AddProductFromForm()
        {
            try
            {
                // Get all input controls
                TextBox txtId = (TextBox)this.Controls.Find("txtId", true)[0];
                ComboBox cmbType = (ComboBox)this.Controls.Find("cmbType", true)[0];
                ComboBox cmbCut = (ComboBox)this.Controls.Find("cmbCut", true)[0];
                TextBox txtColor = (TextBox)this.Controls.Find("txtColor", true)[0];
                ComboBox cmbFabric = (ComboBox)this.Controls.Find("cmbFabric", true)[0];
                ComboBox cmbSize = (ComboBox)this.Controls.Find("cmbSize", true)[0];
                TextBox txtBrand = (TextBox)this.Controls.Find("txtBrand", true)[0];
                NumericUpDown numPrice = (NumericUpDown)this.Controls.Find("numPrice", true)[0];
                
                // Create new product
                Product product = new Product(
                    txtId.Text,
                    (ClothingType)cmbType.SelectedItem,
                    (Cut)cmbCut.SelectedItem,
                    txtColor.Text,
                    (Fabric)cmbFabric.SelectedItem,
                    (Size)cmbSize.SelectedItem,
                    txtBrand.Text,
                    numPrice.Value
                );
                
                // Add to shop
                shop.AddProduct(product);
                
                // Refresh display
                RefreshProductGrid();
                
                // Clear inputs
                txtId.Text = "";
                txtColor.Text = "";
                txtBrand.Text = "";
                numPrice.Value = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteSelectedProduct()
        {
            DataGridView grid = (DataGridView)this.Controls.Find("productGrid", true)[0];
            if (grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a product to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            string id = grid.SelectedRows[0].Cells["Id"].Value.ToString();
            if (shop.DeleteProduct(id))
            {
                RefreshProductGrid();
                MessageBox.Show("Product deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Failed to delete product.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ModifySelectedProduct()
        {
            DataGridView grid = (DataGridView)this.Controls.Find("productGrid", true)[0];
            if (grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a product to modify.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            string id = grid.SelectedRows[0].Cells["Id"].Value.ToString();
            Product selectedProduct = shop.GetProductsByCriteria().Find(p => p.Id == id);
            
            if (selectedProduct == null)
            {
                MessageBox.Show("Product not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            // Open modify form
            ModifyProductForm modifyForm = new ModifyProductForm(selectedProduct);
            if (modifyForm.ShowDialog() == DialogResult.OK)
            {
                Product modifiedProduct = modifyForm.GetModifiedProduct();
                shop.ModifyProduct(id, modifiedProduct);
                RefreshProductGrid();
            }
        }

        private void RefreshProductGrid()
        {
            DataGridView grid = (DataGridView)this.Controls.Find("productGrid", true)[0];
            grid.Rows.Clear();
            
            currentDisplayedProducts = shop.GetProductsByCriteria();
            foreach (var product in currentDisplayedProducts)
            {
                grid.Rows.Add(
                    product.Id,
                    product.Type,
                    product.Cut,
                    product.Color,
                    product.Fabric,
                    product.Size,
                    product.Brand,
                    product.BasePrice.ToString("C"),
                    product.CalculatePrice().ToString("C")
                );
            }
        }

        private void SortProducts()
        {
            ComboBox cmbSortBy = (ComboBox)this.Controls.Find("cmbSortBy", true)[0];
            ComboBox cmbDirection = (ComboBox)this.Controls.Find("cmbDirection", true)[0];
            
            string sortBy = cmbSortBy.SelectedItem.ToString();
            bool ascending = cmbDirection.SelectedItem.ToString() == "Ascending";
            
            // Implement custom sorting (not using Array.Sort or List.Sort)
            switch (sortBy)
            {
                case "Type":
                    BubbleSortProducts((p1, p2) => 
                        ascending ? p1.Type.CompareTo(p2.Type) : p2.Type.CompareTo(p1.Type));
                    break;
                case "Color":
                    BubbleSortProducts((p1, p2) => 
                        ascending ? p1.Color.CompareTo(p2.Color) : p2.Color.CompareTo(p1.Color));
                    break;
                case "Price":
                    BubbleSortProducts((p1, p2) => 
                        ascending ? p1.CalculatePrice().CompareTo(p2.CalculatePrice()) : 
                                   p2.CalculatePrice().CompareTo(p1.CalculatePrice()));
                    break;
            }
            
            RefreshProductGrid();
        }

        // Custom bubble sort implementation
        private void BubbleSortProducts(Comparison<Product> comparison)
        {
            for (int i = 0; i < currentDisplayedProducts.Count - 1; i++)
            {
                for (int j = 0; j < currentDisplayedProducts.Count - i - 1; j++)
                {
                    if (comparison(currentDisplayedProducts[j], currentDisplayedProducts[j + 1]) > 0)
                    {
                        // Swap
                        Product temp = currentDisplayedProducts[j];
                        currentDisplayedProducts[j] = currentDisplayedProducts[j + 1];
                        currentDisplayedProducts[j + 1] = temp;
                    }
                }
            }
        }

        private void NewShop()
        {
            shop = new ClothingShop();
            currentDisplayedProducts.Clear();
            RefreshProductGrid();
            currentFilePath = string.Empty;
        }

        private void OpenShop()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    shop.LoadFromFile(openFileDialog.FileName);
                    currentFilePath = openFileDialog.FileName;
                    RefreshProductGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SaveShop()
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveShopAs();
            }
            else
            {
                try
                {
                    shop.SaveToFile(currentFilePath);
                    MessageBox.Show("Shop saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SaveShopAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                currentFilePath = saveFileDialog.FileName;
                SaveShop();
            }
        }
    }

    // Form for modifying products
    public class ModifyProductForm : Form
    {
        private Product originalProduct;
        private Product modifiedProduct;
        
        public ModifyProductForm(Product product)
        {
            originalProduct = product;
            modifiedProduct = new Product(
                product.Id, product.Type, product.Cut, product.Color, 
                product.Fabric, product.Size, product.Brand, product.BasePrice
            );
            
            InitializeForm();
        }
        
        private void InitializeForm()
        {
            this.Text = "Modify Product";
            this.Size = new Size(400, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            // Create controls similar to main form but populated with existing values
            int yPos = 20;
            int labelWidth = 100;
            int controlWidth = 200;
            int spacing = 40;
            
            // ID
            Label lblId = new Label { Text = "ID:", Left = 20, Top = yPos, Width = labelWidth };
            TextBox txtId = new TextBox { Name = "txtId", Left = labelWidth + 25, Top = yPos, Width = controlWidth, Text = originalProduct.Id };
            txtId.ReadOnly = true; // ID shouldn't be changed
            this.Controls.Add(lblId);
            this.Controls.Add(txtId);
            yPos += spacing;
            
            // Type
            Label lblType = new Label { Text = "Type:", Left = 20, Top = yPos, Width = labelWidth };
            ComboBox cmbType = new ComboBox { Name = "cmbType", Left = labelWidth + 25, Top = yPos, Width = controlWidth };
            cmbType.DataSource = Enum.GetValues(typeof(ClothingType));
            cmbType.SelectedItem = originalProduct.Type;
            this.Controls.Add(lblType);
            this.Controls.Add(cmbType);
            yPos += spacing;
            
            // Cut
            Label lblCut = new Label { Text = "Cut:", Left = 20, Top = yPos, Width = labelWidth };
            ComboBox cmbCut = new ComboBox { Name = "cmbCut", Left = labelWidth + 25, Top = yPos, Width = controlWidth };
            cmbCut.DataSource = Enum.GetValues(typeof(Cut));
            cmbCut.SelectedItem = originalProduct.Cut;
            this.Controls.Add(lblCut);
            this.Controls.Add(cmbCut);
            yPos += spacing;
            
            // Color
            Label lblColor = new Label { Text = "Color:", Left = 20, Top = yPos, Width = labelWidth };
            TextBox txtColor = new TextBox { Name = "txtColor", Left = labelWidth + 25, Top = yPos, Width = controlWidth, Text = originalProduct.Color };
            this.Controls.Add(lblColor);
            this.Controls.Add(txtColor);
            yPos += spacing;
            
            // Fabric
            Label lblFabric = new Label { Text = "Fabric:", Left = 20, Top = yPos, Width = labelWidth };
            ComboBox cmbFabric = new ComboBox { Name = "cmbFabric", Left = labelWidth + 25, Top = yPos, Width = controlWidth };
            cmbFabric.DataSource = Enum.GetValues(typeof(Fabric));
            cmbFabric.SelectedItem = originalProduct.Fabric;
            this.Controls.Add(lblFabric);
            this.Controls.Add(cmbFabric);
            yPos += spacing;
            
            // Size
            Label lblSize = new Label { Text = "Size:", Left = 20, Top = yPos, Width = labelWidth };
            ComboBox cmbSize = new ComboBox { Name = "cmbSize", Left = labelWidth + 25, Top = yPos, Width = controlWidth };
            cmbSize.DataSource = Enum.GetValues(typeof(Size));
            cmbSize.SelectedItem = originalProduct.Size;
            this.Controls.Add(lblSize);
            this.Controls.Add(cmbSize);
            yPos += spacing;
            
            // Brand
            Label lblBrand = new Label { Text = "Brand:", Left = 20, Top = yPos, Width = labelWidth };
            TextBox txtBrand = new TextBox { Name = "txtBrand", Left = labelWidth + 25, Top = yPos, Width = controlWidth, Text = originalProduct.Brand };
            this.Controls.Add(lblBrand);
            this.Controls.Add(txtBrand);
            yPos += spacing;
            
            // Base Price
            Label lblPrice = new Label { Text = "Base Price:", Left = 20, Top = yPos, Width = labelWidth };
            NumericUpDown numPrice = new NumericUpDown { Name = "numPrice", Left = labelWidth + 25, Top = yPos, Width = controlWidth, 
                DecimalPlaces = 2, Minimum = 0, Maximum = 10000, Value = originalProduct.BasePrice };
            this.Controls.Add(lblPrice);
            this.Controls.Add(numPrice);
            yPos += spacing;
            
            // Buttons
            Button btnOK = new Button { Text = "OK", Left = labelWidth + 25, Top = yPos, Width = 80, DialogResult = DialogResult.OK };
            btnOK.Click += (s, e) => 
            {
                // Update modified product with form values
                modifiedProduct.Type = (ClothingType)cmbType.SelectedItem;
                modifiedProduct.Cut = (Cut)cmbCut.SelectedItem;
                modifiedProduct.Color = txtColor.Text;
                modifiedProduct.Fabric = (Fabric)cmbFabric.SelectedItem;
                modifiedProduct.Size = (Size)cmbSize.SelectedItem;
                modifiedProduct.Brand = txtBrand.Text;
                modifiedProduct.BasePrice = numPrice.Value;
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            
            Button btnCancel = new Button { Text = "Cancel", Left = labelWidth + 115, Top = yPos, Width = 80, DialogResult = DialogResult.Cancel };
            btnCancel.Click += (s, e) => this.Close();
            
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);
            
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }
        
        public Product GetModifiedProduct()
        {
            return modifiedProduct;
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
