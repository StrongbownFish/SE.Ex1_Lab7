using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ex1
{
    public partial class ProductForm : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DevConn"].ConnectionString;
        private int? selectedProductId = null;

        public ProductForm()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    string query = "SELECT ItemID, ItemName, Size, Price FROM Item ORDER BY ItemID";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvProducts.DataSource = dt;

                    // Format grid columns
                    dgvProducts.Columns["ItemID"].HeaderText = "Product ID";
                    dgvProducts.Columns["ItemName"].HeaderText = "Product Name";
                    dgvProducts.Columns["Size"].HeaderText = "Size/Specification";
                    dgvProducts.Columns["Price"].HeaderText = "Price";
                    dgvProducts.Columns["Price"].DefaultCellStyle.Format = "N2";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading products: " + ex.Message);
                }
            }
        }
        private int GetNextProductId()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ISNULL(MAX(ItemID), 0) + 1 FROM Item";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    return (int)cmd.ExecuteScalar();
                }
                catch (Exception)
                {
                    return 1;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Please enter a product name.");
                txtProductName.Focus();
                return;
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query;
                    SqlCommand cmd;

                    if (selectedProductId.HasValue)
                    {
                        query = @"UPDATE Item 
                             SET ItemName = @name, Size = @size, Price = @price 
                             WHERE ItemID = @id";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", selectedProductId.Value);
                    }
                    else
                    {
                        int nextId = GetNextProductId();
                        query = @"INSERT INTO Item (ItemID, ItemName, Size, Price) 
                             VALUES (@id, @name, @size, @price)";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", nextId);
                    }

                    cmd.Parameters.AddWithValue("@name", txtProductName.Text.Trim());
                    cmd.Parameters.AddWithValue("@size", txtSize.Text.Trim());

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Product saved successfully!");
                    ClearForm();
                    LoadProducts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving product: " + ex.Message);
                }
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!selectedProductId.HasValue)
            {
                MessageBox.Show("Please select a product to delete.");
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        // Check if product is used in any orders
                        string checkQuery = "SELECT COUNT(*) FROM OrderDetail WHERE ItemID = @id";
                        SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                        checkCmd.Parameters.AddWithValue("@id", selectedProductId.Value);
                        int orderCount = (int)checkCmd.ExecuteScalar();

                        if (orderCount > 0)
                        {
                            MessageBox.Show("This product cannot be deleted because it is used in orders.");
                            return;
                        }

                        string deleteQuery = "DELETE FROM Item WHERE ItemID = @id";
                        SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn);
                        deleteCmd.Parameters.AddWithValue("@id", selectedProductId.Value);
                        deleteCmd.ExecuteNonQuery();

                        MessageBox.Show("Product deleted successfully!");
                        ClearForm();
                        LoadProducts();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting product: " + ex.Message);
                    }
                }
            }
        }

        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedProductId = Convert.ToInt32(dgvProducts.Rows[e.RowIndex].Cells["ItemID"].Value);
                txtProductName.Text = dgvProducts.Rows[e.RowIndex].Cells["ItemName"].Value.ToString();
                txtSize.Text = dgvProducts.Rows[e.RowIndex].Cells["Size"].Value.ToString();
                lblPriceValue.Text = Convert.ToDecimal(dgvProducts.Rows[e.RowIndex].Cells["Price"].Value).ToString("N2");
                btnSave.Text = "Update";
                btnDelete.Enabled = true;
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            selectedProductId = null;
            txtProductName.Clear();
            txtSize.Clear();
            lblPriceValue.Text = "0.00";
            btnSave.Text = "Save";
            btnDelete.Enabled = false;
            txtProductName.Focus();
        }
    }
}
