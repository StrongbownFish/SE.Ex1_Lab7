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
                    string query = "SELECT AgentID, AgentName FROM Agent ORDER BY AgentName";
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
                    string query = "SELECT ItemID, ItemName FROM Item ORDER BY ItemName";
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
        private int GetNextOrderId()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ISNULL(MAX(OrderID), 0) + 1 FROM [Order]";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    return (int)cmd.ExecuteScalar();
                }
                catch (Exception)
                {
                    return 1; // Return 1 if there's an error or table is empty
                }
            }
        }
        private int GetNextOrderDetailId()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ISNULL(MAX(ID), 0) + 1 FROM OrderDetail";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    return (int)cmd.ExecuteScalar();
                }
                catch (Exception)
                {
                    return 1;
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
                cboProduct.Focus();
                return;
            }

            if (nudQuantity.Value <= 0)
            {
                MessageBox.Show("Please enter a valid quantity.");
                nudQuantity.Focus();
                return;
            }

            if (nudUnitPrice.Value <= 0)
            {
                MessageBox.Show("Please enter a valid unit price.");
                nudUnitPrice.Focus();
                return;
            }

            // Check if product already exists in the order
            foreach (DataRow row in dtOrderDetails.Rows)
            {
                if (row["ItemID"].ToString() == cboProduct.SelectedValue.ToString())
                {
                    MessageBox.Show("This product is already in the order. Please update the existing entry instead.");
                    return;
                }
            }

            decimal itemTotal = nudQuantity.Value * nudUnitPrice.Value;
            DataRow newRow = dtOrderDetails.NewRow();
            newRow["ItemID"] = cboProduct.SelectedValue;
            newRow["ItemName"] = cboProduct.Text;
            newRow["Quantity"] = nudQuantity.Value;
            newRow["UnitAmount"] = nudUnitPrice.Value;
            newRow["TotalAmount"] = itemTotal;
            dtOrderDetails.Rows.Add(newRow);

            CalculateTotal();
            ClearItemInputs();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cboAgent.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an agent.");
                cboAgent.Focus();
                return;
            }

            if (dtOrderDetails.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one item to the order.");
                cboProduct.Focus();
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    int orderId = selectedOrderId ?? GetNextOrderId();

                    // Insert or Update Order
                    string orderQuery;
                    if (selectedOrderId.HasValue)
                    {
                        orderQuery = "UPDATE [Order] SET AgentID = @agentId, OrderDate = @orderDate WHERE OrderID = @orderId";
                    }
                    else
                    {
                        orderQuery = "INSERT INTO [Order] (OrderID, AgentID, OrderDate) VALUES (@orderId, @agentId, @orderDate)";
                    }

                    using (SqlCommand cmdOrder = new SqlCommand(orderQuery, conn, transaction))
                    {
                        cmdOrder.Parameters.AddWithValue("@orderId", orderId);
                        cmdOrder.Parameters.AddWithValue("@agentId", cboAgent.SelectedValue);
                        cmdOrder.Parameters.AddWithValue("@orderDate", dtpOrderDate.Value);
                        cmdOrder.ExecuteNonQuery();
                    }

                    // Delete existing order details if updating
                    if (selectedOrderId.HasValue)
                    {
                        string deleteDetails = "DELETE FROM OrderDetail WHERE OrderID = @orderId";
                        using (SqlCommand cmdDelete = new SqlCommand(deleteDetails, conn, transaction))
                        {
                            cmdDelete.Parameters.AddWithValue("@orderId", orderId);
                            cmdDelete.ExecuteNonQuery();
                        }
                    }

                    // Get the next available ID for OrderDetail
                    int detailId = GetNextOrderDetailId();

                    // Insert Order Details
                    string detailQuery = @"INSERT INTO OrderDetail (ID, OrderID, ItemID, Quantity, UnitAmount) 
                                     VALUES (@id, @orderId, @itemId, @quantity, @unitAmount)";

                    foreach (DataRow row in dtOrderDetails.Rows)
                    {
                        using (SqlCommand cmdDetail = new SqlCommand(detailQuery, conn, transaction))
                        {
                            cmdDetail.Parameters.AddWithValue("@id", detailId++);  // Use and increment ID for each detail
                            cmdDetail.Parameters.AddWithValue("@orderId", orderId);
                            cmdDetail.Parameters.AddWithValue("@itemId", row["ItemID"]);
                            cmdDetail.Parameters.AddWithValue("@quantity", row["Quantity"]);
                            cmdDetail.Parameters.AddWithValue("@unitAmount", row["UnitAmount"]);
                            cmdDetail.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    MessageBox.Show("Order saved successfully!");

                    // Ask if user wants to print the order
                    if (MessageBox.Show("Would you like to print this order?", "Print Order",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        PrintOrder(orderId);
                    }

                    ClearForm();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error saving order: " + ex.Message);
                }
            }
        }
        private void PrintOrder(int orderId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    // Get order details for printing
                    string query = @"
                    SELECT 
                        O.OrderID,
                        O.OrderDate,
                        A.AgentName,
                        A.Address,
                        I.ItemName,
                        OD.Quantity,
                        OD.UnitAmount,
                        (OD.Quantity * OD.UnitAmount) as TotalAmount
                    FROM [Order] O
                    JOIN Agent A ON O.AgentID = A.AgentID
                    JOIN OrderDetail OD ON O.OrderID = OD.OrderID
                    JOIN Item I ON OD.ItemID = I.ItemID
                    WHERE O.OrderID = @orderId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@orderId", orderId);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dtPrint = new DataTable();
                    adapter.Fill(dtPrint);

                    // Create and show the print preview form
                    PrintOrderForm printForm = new PrintOrderForm(dtPrint);
                    printForm.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error preparing order for print: " + ex.Message);
                }
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!selectedOrderId.HasValue)
            {
                MessageBox.Show("Please select an order to delete.");
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this order?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // Delete order details first
                        string deleteDetails = "DELETE FROM OrderDetail WHERE OrderID = @orderId";
                        using (SqlCommand cmdDelete = new SqlCommand(deleteDetails, conn, transaction))
                        {
                            cmdDelete.Parameters.AddWithValue("@orderId", selectedOrderId.Value);
                            cmdDelete.ExecuteNonQuery();
                        }

                        // Delete order
                        string deleteOrder = "DELETE FROM [Order] WHERE OrderID = @orderId";
                        using (SqlCommand cmdDelete = new SqlCommand(deleteOrder, conn, transaction))
                        {
                            cmdDelete.Parameters.AddWithValue("@orderId", selectedOrderId.Value);
                            cmdDelete.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show("Order deleted successfully!");
                        ClearForm();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error deleting order: " + ex.Message);
                    }
                }
            }
        }

        private void CalculateTotal()
        {
            totalAmount = dtOrderDetails.AsEnumerable()
                .Sum(row => row.Field<decimal>("TotalAmount"));
            lblTotal.Text = $"Total Amount: {totalAmount:C2}";
        }

        private void ClearItemInputs()
        {
            cboProduct.SelectedIndex = -1;
            nudQuantity.Value = 1;
            nudUnitPrice.Value = 0;
            cboProduct.Focus();
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
            btnDelete.Enabled = false;
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
