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
    public partial class OrderForm : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DevConn"].ConnectionString;
        private DataTable dtOrderDetails;
        private int? selectedOrderId = null;
        private decimal totalAmount = 0;

        public OrderForm()
        {
            InitializeComponent();
            InitializeOrderDetails();
            LoadAgents();
            LoadProducts();
            SetupDataGridView();
        }

        private void InitializeOrderDetails()
        {
            dtOrderDetails = new DataTable();
            dtOrderDetails.Columns.Add("ItemID", typeof(int));
            dtOrderDetails.Columns.Add("ItemName", typeof(string));
            dtOrderDetails.Columns.Add("Quantity", typeof(int));
            dtOrderDetails.Columns.Add("UnitAmount", typeof(decimal));
            dtOrderDetails.Columns.Add("TotalAmount", typeof(decimal));
            dgvOrderDetails.DataSource = dtOrderDetails;
        }

        private void LoadAgents()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT AgentID, AgentName FROM Agent";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    cboAgent.DataSource = dt;
                    cboAgent.DisplayMember = "AgentName";
                    cboAgent.ValueMember = "AgentID";
                    cboAgent.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading agents: " + ex.Message);
                }
            }
        }

        private void LoadProducts()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ItemID, ItemName FROM Item";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    cboProduct.DataSource = dt;
                    cboProduct.DisplayMember = "ItemName";
                    cboProduct.ValueMember = "ItemID";
                    cboProduct.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading products: " + ex.Message);
                }
            }
        }

        private void SetupDataGridView()
        {
            dgvOrderDetails.Columns["ItemID"].Visible = false;
            dgvOrderDetails.Columns["ItemName"].HeaderText = "Product";
            dgvOrderDetails.Columns["Quantity"].HeaderText = "Quantity";
            dgvOrderDetails.Columns["UnitAmount"].HeaderText = "Unit Price";
            dgvOrderDetails.Columns["TotalAmount"].HeaderText = "Total";

            dgvOrderDetails.Columns["UnitAmount"].DefaultCellStyle.Format = "C2";
            dgvOrderDetails.Columns["TotalAmount"].DefaultCellStyle.Format = "C2";
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            if (cboProduct.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a product.");
                return;
            }

            if (nudQuantity.Value <= 0)
            {
                MessageBox.Show("Please enter a valid quantity.");
                return;
            }

            if (nudUnitPrice.Value <= 0)
            {
                MessageBox.Show("Please enter a valid unit price.");
                return;
            }

            DataRow row = dtOrderDetails.NewRow();
            row["ItemID"] = cboProduct.SelectedValue;
            row["ItemName"] = cboProduct.Text;
            row["Quantity"] = nudQuantity.Value;
            row["UnitAmount"] = nudUnitPrice.Value;
            row["TotalAmount"] = nudQuantity.Value * nudUnitPrice.Value;
            dtOrderDetails.Rows.Add(row);

            CalculateTotal();
            ClearItemInputs();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cboAgent.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an agent.");
                return;
            }

            if (dtOrderDetails.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one item to the order.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Insert or Update Order
                    string orderQuery;
                    if (selectedOrderId.HasValue)
                    {
                        orderQuery = @"UPDATE [Order] SET AgentID = @agentId, OrderDate = @orderDate 
                                     WHERE OrderID = @orderId";
                    }
                    else
                    {
                        orderQuery = @"INSERT INTO [Order] (AgentID, OrderDate) 
                                     VALUES (@agentId, @orderDate); 
                                     SELECT SCOPE_IDENTITY();";
                    }

                    SqlCommand cmdOrder = new SqlCommand(orderQuery, conn, transaction);
                    cmdOrder.Parameters.AddWithValue("@agentId", cboAgent.SelectedValue);
                    cmdOrder.Parameters.AddWithValue("@orderDate", dtpOrderDate.Value);

                    int orderId;
                    if (selectedOrderId.HasValue)
                    {
                        cmdOrder.Parameters.AddWithValue("@orderId", selectedOrderId.Value);
                        cmdOrder.ExecuteNonQuery();
                        orderId = selectedOrderId.Value;

                        // Delete existing order details
                        string deleteDetails = "DELETE FROM OrderDetail WHERE OrderID = @orderId";
                        SqlCommand cmdDelete = new SqlCommand(deleteDetails, conn, transaction);
                        cmdDelete.Parameters.AddWithValue("@orderId", orderId);
                        cmdDelete.ExecuteNonQuery();
                    }
                    else
                    {
                        orderId = Convert.ToInt32(cmdOrder.ExecuteScalar());
                    }

                    // Insert Order Details
                    string detailQuery = @"INSERT INTO OrderDetail (OrderID, ItemID, Quantity, UnitAmount) 
                                         VALUES (@orderId, @itemId, @quantity, @unitAmount)";

                    foreach (DataRow row in dtOrderDetails.Rows)
                    {
                        SqlCommand cmdDetail = new SqlCommand(detailQuery, conn, transaction);
                        cmdDetail.Parameters.AddWithValue("@orderId", orderId);
                        cmdDetail.Parameters.AddWithValue("@itemId", row["ItemID"]);
                        cmdDetail.Parameters.AddWithValue("@quantity", row["Quantity"]);
                        cmdDetail.Parameters.AddWithValue("@unitAmount", row["UnitAmount"]);
                        cmdDetail.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    MessageBox.Show("Order saved successfully!");
                    ClearForm();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error saving order: " + ex.Message);
                }
            }
        }

        private void CalculateTotal()
        {
            totalAmount = 0;
            foreach (DataRow row in dtOrderDetails.Rows)
            {
                totalAmount += Convert.ToDecimal(row["TotalAmount"]);
            }
            lblTotal.Text = $"Total Amount: {totalAmount:C2}";
        }

        private void ClearItemInputs()
        {
            cboProduct.SelectedIndex = -1;
            nudQuantity.Value = 1;
            nudUnitPrice.Value = 0;
        }

        private void ClearForm()
        {
            selectedOrderId = null;
            cboAgent.SelectedIndex = -1;
            dtpOrderDate.Value = DateTime.Now;
            dtOrderDetails.Clear();
            ClearItemInputs();
            totalAmount = 0;
            lblTotal.Text = "Total Amount: $0.00";
            btnSave.Text = "Save Order";
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void dgvOrderDetails_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove this item?", "Confirm Delete",
                MessageBoxButtons.YesNo) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void dgvOrderDetails_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            CalculateTotal();
        }
    }
}
