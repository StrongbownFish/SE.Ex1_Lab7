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
    public partial class OrderDetail : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DevConn"].ConnectionString;

        public OrderDetail()
        {
            InitializeComponent();
            LoadOrders();
        }
        private void LoadOrders()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    string query = @"
                        SELECT 
                            O.OrderID,
                            O.OrderDate,
                            A.AgentName,
                            COUNT(OD.ItemID) as NumberOfItems,
                            SUM(OD.Quantity * I.Price) as TotalAmount
                        FROM [Order] O
                        JOIN Agent A ON O.AgentID = A.AgentID
                        JOIN OrderDetail OD ON O.OrderID = OD.OrderID
                        JOIN Item I ON OD.ItemID = I.ItemID
                        GROUP BY O.OrderID, O.OrderDate, A.AgentName
                        ORDER BY O.OrderDate DESC";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvOrders.DataSource = dt;

                    // Format columns
                    dgvOrders.Columns["OrderDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    dgvOrders.Columns["TotalAmount"].DefaultCellStyle.Format = "N2";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading orders: " + ex.Message);
                }
            }
        }

        private void LoadOrderDetails(int orderId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    string query = @"
                        SELECT 
                            I.ItemName as 'Product',
                            OD.Quantity,
                            I.Price as 'Unit Price',
                            (OD.Quantity * I.Price) as 'Total'
                        FROM OrderDetail OD
                        JOIN Item I ON OD.ItemID = I.ItemID
                        WHERE OD.OrderID = @OrderID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvOrderDetails.DataSource = dt;

                    // Format columns
                    dgvOrderDetails.Columns["Unit Price"].DefaultCellStyle.Format = "N2";
                    dgvOrderDetails.Columns["Total"].DefaultCellStyle.Format = "N2";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading order details: " + ex.Message);
                }
            }
        }

        private void dgvOrders_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int orderId = Convert.ToInt32(dgvOrders.Rows[e.RowIndex].Cells["OrderID"].Value);
                LoadOrderDetails(orderId);
            }
        }
    }
}
