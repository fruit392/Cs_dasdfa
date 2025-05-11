using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace ClothingShopUI
{
    public class MainForm : Form
    {
        private ClothingShop shop = new ClothingShop();
        private List<Product> displayedProducts = new List<Product>();
        private string currentFilePath = string.Empty;

        // UI Controls
        private DataGridView productsGridView;
        private ComboBox sortFieldComboBox;
        private ComboBox sortDirectionComboBox;
        private Button sortButton;
        private Button addButton;
        private Button editButton;
        private Button deleteButton;
        private Button saveButton;
        private Button loadButton;
        private TextBox searchTextBox;
        private ComboBox filterTypeComboBox;
        private ComboBox filterSizeComboBox;
        private ComboBox filterColorComboBox;
        private Button filterButton;
        private Button clearFilterButton;

        public MainForm()
        {
            InitializeUI();
            LoadEnumsIntoComboBoxes();
            RefreshProductList();
        }

        private void InitializeUI()
        {
            this.Text = "Clothing Shop Management";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Main Table Layout
            var mainTable = new TableLayoutPanel();
            mainTable.Dock = DockStyle.Fill;
            mainTable.ColumnCount = 1;
            mainTable.RowCount = 3;
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            this.Controls.Add(mainTable);

            // Top Panel - Controls
            var topPanel = new Panel();
            topPanel.Dock = DockStyle.Fill;
            mainTable.Controls.Add(topPanel, 0, 0);

            // Filter Controls
            var filterGroup = new GroupBox();
            filterGroup.Text = "Filter Products";
            filterGroup.Location = new Point(10, 10);
            filterGroup.Size = new Size(400, 100);
            topPanel.Controls.Add(filterGroup);

            filterTypeComboBox = new ComboBox();
            filterTypeComboBox.Location = new Point(10, 20);
            filterTypeComboBox.Size = new Size(120, 25);
            filterTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            filterGroup.Controls.Add(filterTypeComboBox);

            filterSizeComboBox = new ComboBox();
            filterSizeComboBox.Location = new Point(140, 20);
            filterSizeComboBox.Size = new Size(80, 25);
            filterSizeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            filterGroup.Controls.Add(filterSizeComboBox);

            filterColorComboBox = new ComboBox();
            filterColorComboBox.Location = new Point(230, 20);
            filterColorComboBox.Size = new Size(100, 25);
            filterGroup.Controls.Add(filterColorComboBox);

            filterButton = new Button();
            filterButton.Text = "Apply Filter";
            filterButton.Location = new Point(340, 20);
            filterButton.Size = new Size(50, 25);
            filterButton.Click += FilterButton_Click;
            filterGroup.Controls.Add(filterButton);

            clearFilterButton = new Button();
            clearFilterButton.Text = "Clear";
            clearFilterButton.Location = new Point(340, 50);
            clearFilterButton.Size = new Size(50, 25);
            clearFilterButton.Click += ClearFilterButton_Click;
            filterGroup.Controls.Add(clearFilterButton);

            // Sort Controls
            var sortGroup = new GroupBox();
            sortGroup.Text = "Sort Products";
            sortGroup.Location = new Point(420, 10);
            sortGroup.Size = new Size(300, 100);
            topPanel.Controls.Add(sortGroup);

            sortFieldComboBox = new ComboBox();
            sortFieldComboBox.Location = new Point(10, 20);
            sortFieldComboBox.Size = new Size(120, 25);
            sortFieldComboBox.Items.AddRange(new string[] { "Type", "Color", "Price" });
            sortFieldComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            sortGroup.Controls.Add(sortFieldComboBox);

            sortDirectionComboBox = new ComboBox();
            sortDirectionComboBox.Location = new Point(140, 20);
            sortDirectionComboBox.Size = new Size(100, 25);
            sortDirectionComboBox.Items.AddRange(new string[] { "Ascending", "Descending" });
            sortDirectionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            sortDirectionComboBox.SelectedIndex = 0;
            sortGroup.Controls.Add(sortDirectionComboBox);

            sortButton = new Button();
            sortButton.Text = "Sort";
            sortButton.Location = new Point(250, 20);
            sortButton.Size = new Size(40, 25);
            sortButton.Click += SortButton_Click;
            sortGroup.Controls.Add(sortButton);

            // Action Buttons
            var actionGroup = new GroupBox();
            actionGroup.Text = "Actions";
            actionGroup.Location = new Point(730, 10);
            actionGroup.Size = new Size(250, 100);
            topPanel.Controls.Add(actionGroup);

            addButton = new Button();
            addButton.Text = "Add";
            addButton.Location = new Point(10, 20);
            addButton.Size = new Size(70, 25);
            addButton.Click += AddButton_Click;
            actionGroup.Controls.Add(addButton);

            editButton = new Button();
            editButton.Text = "Edit";
            editButton.Location = new Point(90, 20);
            editButton.Size = new Size(70, 25);
            editButton.Click += EditButton_Click;
            actionGroup.Controls.Add(editButton);

            deleteButton = new Button();
            deleteButton.Text = "Delete";
            deleteButton.Location = new Point(170, 20);
            deleteButton.Size = new Size(70, 25);
            deleteButton.Click += DeleteButton_Click;
            actionGroup.Controls.Add(deleteButton);

            saveButton = new Button();
            saveButton.Text = "Save";
            saveButton.Location = new Point(10, 50);
            saveButton.Size = new Size(70, 25);
            saveButton.Click += SaveButton_Click;
            actionGroup.Controls.Add(saveButton);

            loadButton = new Button();
            loadButton.Text = "Load";
            loadButton.Location = new Point(90, 50);
            loadButton.Size = new Size(70, 25);
            loadButton.Click += LoadButton_Click;
            actionGroup.Controls.Add(loadButton);

            // Search
            var searchGroup = new GroupBox();
            searchGroup.Text = "Search";
            searchGroup.Location = new Point(10, 120);
            searchGroup.Size = new Size(970, 60);
            topPanel.Controls.Add(searchGroup);

            searchTextBox = new TextBox();
            searchTextBox.Location = new Point(10, 20);
            searchTextBox.Size = new Size(850, 25);
            searchTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            searchTextBox.TextChanged += SearchTextBox_TextChanged;
            searchGroup.Controls.Add(searchTextBox);

            // Products Grid
            productsGridView = new DataGridView();
            productsGridView.Dock = DockStyle.Fill;
            productsGridView.AutoGenerateColumns = false;
            productsGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            productsGridView.MultiSelect = false;
            productsGridView.ReadOnly = true;
            productsGridView.AllowUserToAddRows = false;
            productsGridView.AllowUserToDeleteRows = false;

            // Add columns
            productsGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Id",
                HeaderText = "ID",
                Width = 80
            });

            productsGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Type",
                HeaderText = "Type",
                Width = 100
            });

            productsGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Cut",
                HeaderText = "Cut",
                Width = 80
            });

            productsGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Color",
                HeaderText = "Color",
                Width = 100
            });

            productsGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Fabric",
                HeaderText = "Fabric",
                Width = 80
            });

            productsGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Size",
                HeaderText = "Size",
                Width = 50
            });

            productsGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Brand",
                HeaderText = "Brand",
                Width = 100
            });

            productsGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "BasePrice",
                HeaderText = "Base Price",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle() { Format = "C2" }
            });

            productsGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "CalculatePrice",
                HeaderText = "Final Price",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle() { Format = "C2" }
            });

            mainTable.Controls.Add(productsGridView, 0, 2);
        }

        private void LoadEnumsIntoComboBoxes()
        {
            // Load types
            filterTypeComboBox.Items.Add("All Types");
            foreach (ClothingType type in Enum.GetValues(typeof(ClothingType)))
            {
                filterTypeComboBox.Items.Add(type);
            }
            filterTypeComboBox.SelectedIndex = 0;

            // Load sizes
            filterSizeComboBox.Items.Add("All Sizes");
            foreach (Size size in Enum.GetValues(typeof(Size)))
            {
                filterSizeComboBox.Items.Add(size);
            }
            filterSizeComboBox.SelectedIndex = 0;

            // Load colors
            filterColorComboBox.Items.Add("All Colors");
            filterColorComboBox.Items.Add("black");
            filterColorComboBox.Items.Add("white");
            filterColorComboBox.Items.Add("blue");
            filterColorComboBox.Items.Add("red");
            filterColorComboBox.Items.Add("green");
            filterColorComboBox.Items.Add("gold");
            filterColorComboBox.Items.Add("silver");
            filterColorComboBox.SelectedIndex = 0;
        }

        private void RefreshProductList()
        {
            displayedProducts.Clear();
            foreach (var product in shop.GetProductsByCriteria())
            {
                displayedProducts.Add(product);
            }
            productsGridView.DataSource = null;
            productsGridView.DataSource = displayedProducts;
        }

        private void SortButton_Click(object sender, EventArgs e)
        {
            if (sortFieldComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a field to sort by", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string field = sortFieldComboBox.SelectedItem.ToString();
            bool ascending = sortDirectionComboBox.SelectedItem.ToString() == "Ascending";

            // Implement custom sorting (bubble sort in this case)
            for (int i = 0; i < displayedProducts.Count - 1; i++)
            {
                for (int j = 0; j < displayedProducts.Count - i - 1; j++)
                {
                    bool swap = false;
                    
                    switch (field)
                    {
                        case "Type":
                            if (ascending)
                                swap = displayedProducts[j].Type > displayedProducts[j + 1].Type;
                            else
                                swap = displayedProducts[j].Type < displayedProducts[j + 1].Type;
                            break;
                            
                        case "Color":
                            if (ascending)
                                swap = string.Compare(displayedProducts[j].Color, displayedProducts[j + 1].Color) > 0;
                            else
                                swap = string.Compare(displayedProducts[j].Color, displayedProducts[j + 1].Color) < 0;
                            break;
                            
                        case "Price":
                            if (ascending)
                                swap = displayedProducts[j].CalculatePrice() > displayedProducts[j + 1].CalculatePrice();
                            else
                                swap = displayedProducts[j].CalculatePrice() < displayedProducts[j + 1].CalculatePrice();
                            break;
                    }

                    if (swap)
                    {
                        var temp = displayedProducts[j];
                        displayedProducts[j] = displayedProducts[j + 1];
                        displayedProducts[j + 1] = temp;
                    }
                }
            }

            productsGridView.DataSource = null;
            productsGridView.DataSource = displayedProducts;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            var form = new ProductForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    shop.AddProduct(form.Product);
                    RefreshProductList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (productsGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a product to edit", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedProduct = (Product)productsGridView.SelectedRows[0].DataBoundItem;
            var form = new ProductForm(selectedProduct);
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    shop.ModifyProduct(selectedProduct.Id, form.Product);
                    RefreshProductList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (productsGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a product to delete", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedProduct = (Product)productsGridView.SelectedRows[0].DataBoundItem;
            if (MessageBox.Show($"Are you sure you want to delete product {selectedProduct.Id}?", 
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                shop.DeleteProduct(selectedProduct.Id);
                RefreshProductList();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Text Files|*.txt|All Files|*.*";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    shop.SaveToFile(saveDialog.FileName);
                    currentFilePath = saveDialog.FileName;
                    MessageBox.Show("Products saved successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Filter = "Text Files|*.txt|All Files|*.*";
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    shop.LoadFromFile(openDialog.FileName);
                    currentFilePath = openDialog.FileName;
                    RefreshProductList();
                    MessageBox.Show("Products loaded successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            string searchText = searchTextBox.Text.ToLower();
            var filtered = new List<Product>();

            foreach (var product in shop.GetProductsByCriteria())
            {
                if (product.Id.ToLower().Contains(searchText) ||
                    product.Type.ToString().ToLower().Contains(searchText) ||
                    product.Color.ToLower().Contains(searchText) ||
                    product.Brand.ToLower().Contains(searchText) ||
                    product.Fabric.ToString().ToLower().Contains(searchText))
                {
                    filtered.Add(product);
                }
            }

            displayedProducts = filtered;
            productsGridView.DataSource = null;
            productsGridView.DataSource = displayedProducts;
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            ClothingType? typeFilter = null;
            if (filterTypeComboBox.SelectedIndex > 0)
            {
                typeFilter = (ClothingType)filterTypeComboBox.SelectedItem;
            }

            Size? sizeFilter = null;
            if (filterSizeComboBox.SelectedIndex > 0)
            {
                sizeFilter = (Size)filterSizeComboBox.SelectedItem;
            }

            string colorFilter = null;
            if (filterColorComboBox.SelectedIndex > 0)
            {
                colorFilter = filterColorComboBox.SelectedItem.ToString();
            }

            displayedProducts = shop.GetProductsByCriteria(sizeFilter, typeFilter, colorFilter);
            productsGridView.DataSource = null;
            productsGridView.DataSource = displayedProducts;
        }

        private void ClearFilterButton_Click(object sender, EventArgs e)
        {
            filterTypeComboBox.SelectedIndex = 0;
            filterSizeComboBox.SelectedIndex = 0;
            filterColorComboBox.SelectedIndex = 0;
            RefreshProductList();
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    public class ProductForm : Form
    {
        public Product Product { get; private set; }

        private TextBox idTextBox;
        private ComboBox typeComboBox;
        private ComboBox cutComboBox;
        private TextBox colorTextBox;
        private ComboBox fabricComboBox;
        private ComboBox sizeComboBox;
        private TextBox brandTextBox;
        private TextBox basePriceTextBox;
        private Button okButton;
        private Button cancelButton;

        public ProductForm(Product productToEdit = null)
        {
            InitializeUI();
            
            if (productToEdit != null)
            {
                Text = "Edit Product";
                idTextBox.Text = productToEdit.Id;
                typeComboBox.SelectedItem = productToEdit.Type;
                cutComboBox.SelectedItem = productToEdit.Cut;
                colorTextBox.Text = productToEdit.Color;
                fabricComboBox.SelectedItem = productToEdit.Fabric;
                sizeComboBox.SelectedItem = productToEdit.Size;
                brandTextBox.Text = productToEdit.Brand;
                basePriceTextBox.Text = productToEdit.BasePrice.ToString("0.00");
            }
            else
            {
                Text = "Add New Product";
                typeComboBox.SelectedIndex = 0;
                cutComboBox.SelectedIndex = 0;
                fabricComboBox.SelectedIndex = 0;
                sizeComboBox.SelectedIndex = 0;
            }
        }

        private void InitializeUI()
        {
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var tableLayout = new TableLayoutPanel();
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.ColumnCount = 2;
            tableLayout.RowCount = 10;
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            this.Controls.Add(tableLayout);

            // ID
            tableLayout.Controls.Add(new Label() { Text = "ID:", TextAlign = ContentAlignment.MiddleRight }, 0, 0);
            idTextBox = new TextBox();
            tableLayout.Controls.Add(idTextBox, 1, 0);

            // Type
            tableLayout.Controls.Add(new Label() { Text = "Type:", TextAlign = ContentAlignment.MiddleRight }, 0, 1);
            typeComboBox = new ComboBox();
            typeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (ClothingType type in Enum.GetValues(typeof(ClothingType)))
            {
                typeComboBox.Items.Add(type);
            }
            tableLayout.Controls.Add(typeComboBox, 1, 1);

            // Cut
            tableLayout.Controls.Add(new Label() { Text = "Cut:", TextAlign = ContentAlignment.MiddleRight }, 0, 2);
            cutComboBox = new ComboBox();
            cutComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (Cut cut in Enum.GetValues(typeof(Cut)))
            {
                cutComboBox.Items.Add(cut);
            }
            tableLayout.Controls.Add(cutComboBox, 1, 2);

            // Color
            tableLayout.Controls.Add(new Label() { Text = "Color:", TextAlign = ContentAlignment.MiddleRight }, 0, 3);
            colorTextBox = new TextBox();
            tableLayout.Controls.Add(colorTextBox, 1, 3);

            // Fabric
            tableLayout.Controls.Add(new Label() { Text = "Fabric:", TextAlign = ContentAlignment.MiddleRight }, 0, 4);
            fabricComboBox = new ComboBox();
            fabricComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (Fabric fabric in Enum.GetValues(typeof(Fabric)))
            {
                fabricComboBox.Items.Add(fabric);
            }
            tableLayout.Controls.Add(fabricComboBox, 1, 4);

            // Size
            tableLayout.Controls.Add(new Label() { Text = "Size:", TextAlign = ContentAlignment.MiddleRight }, 0, 5);
            sizeComboBox = new ComboBox();
            sizeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (Size size in Enum.GetValues(typeof(Size)))
            {
                sizeComboBox.Items.Add(size);
            }
            tableLayout.Controls.Add(sizeComboBox, 1, 5);

            // Brand
            tableLayout.Controls.Add(new Label() { Text = "Brand:", TextAlign = ContentAlignment.MiddleRight }, 0, 6);
            brandTextBox = new TextBox();
            tableLayout.Controls.Add(brandTextBox, 1, 6);

            // Base Price
            tableLayout.Controls.Add(new Label() { Text = "Base Price:", TextAlign = ContentAlignment.MiddleRight }, 0, 7);
            basePriceTextBox = new TextBox();
            tableLayout.Controls.Add(basePriceTextBox, 1, 7);

            // Buttons
            okButton = new Button() { Text = "OK", DialogResult = DialogResult.OK };
            okButton.Click += OkButton_Click;
            tableLayout.Controls.Add(okButton, 0, 9);

            cancelButton = new Button() { Text = "Cancel", DialogResult = DialogResult.Cancel };
            tableLayout.Controls.Add(cancelButton, 1, 9);

            // Set anchors
            foreach (Control control in tableLayout.Controls)
            {
                control.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(idTextBox.Text))
            {
                MessageBox.Show("Please enter an ID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(colorTextBox.Text))
            {
                MessageBox.Show("Please enter a color", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(brandTextBox.Text))
            {
                MessageBox.Show("Please enter a brand", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!decimal.TryParse(basePriceTextBox.Text, out decimal basePrice) || basePrice <= 0)
            {
                MessageBox.Show("Please enter a valid positive price", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create product
            Product = new Product(
                idTextBox.Text,
                (ClothingType)typeComboBox.SelectedItem,
                (Cut)cutComboBox.SelectedItem,
                colorTextBox.Text,
                (Fabric)fabricComboBox.SelectedItem,
                (Size)sizeComboBox.SelectedItem,
                brandTextBox.Text,
                basePrice
            );

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
