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
    public partial class FilterForm : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DevConn"].ConnectionString;

        public FilterForm()
        {
            InitializeComponent();
            // Load best items by default when form opens
            ShowBestItems();
        }

        private void ShowBestItems()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                    SELECT 
                        I.ItemName as 'Product Name',
                        SUM(OD.Quantity) as 'Total Quantity Sold',
                        COUNT(DISTINCT O.OrderID) as 'Number of Orders',
                        SUM(OD.Quantity * OD.UnitAmount) as 'Total Revenue'
                    FROM Item I
                    LEFT JOIN OrderDetail OD ON I.ItemID = OD.ItemID
                    LEFT JOIN [Order] O ON OD.OrderID = O.OrderID
                    GROUP BY I.ItemID, I.ItemName
                    ORDER BY SUM(OD.Quantity) DESC";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvResults.DataSource = dt;

                    FormatCurrencyColumn("Total Revenue");
                    lblTitle.Text = "Best Selling Items";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading best items: " + ex.Message);
            }
        }

        private void ShowItemsByCustomer()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                    SELECT 
                        A.AgentName as 'Customer Name',
                        I.ItemName as 'Product Name',
                        SUM(OD.Quantity) as 'Total Quantity',
                        SUM(OD.Quantity * OD.UnitAmount) as 'Total Amount',
                        MAX(O.OrderDate) as 'Last Purchase Date'
                    FROM Agent A
                    JOIN [Order] O ON A.AgentID = O.AgentID
                    JOIN OrderDetail OD ON O.OrderID = OD.OrderID
                    JOIN Item I ON OD.ItemID = I.ItemID
                    GROUP BY A.AgentID, A.AgentName, I.ItemID, I.ItemName
                    ORDER BY A.AgentName, SUM(OD.Quantity) DESC";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvResults.DataSource = dt;

                    FormatCurrencyColumn("Total Amount");
                    FormatDateColumn("Last Purchase Date");
                    lblTitle.Text = "Items Purchased by Customer";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading items by customer: " + ex.Message);
            }
        }

        private void ShowCustomerPurchases()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                    SELECT 
                        A.AgentName as 'Customer Name',
                        COUNT(DISTINCT O.OrderID) as 'Total Orders',
                        SUM(OD.Quantity * OD.UnitAmount) as 'Total Purchase Amount',
                        MIN(O.OrderDate) as 'First Purchase',
                        MAX(O.OrderDate) as 'Last Purchase',
                        AVG(OD.Quantity * OD.UnitAmount) as 'Average Order Value'
                    FROM Agent A
                    LEFT JOIN [Order] O ON A.AgentID = O.AgentID
                    LEFT JOIN OrderDetail OD ON O.OrderID = OD.OrderID
                    GROUP BY A.AgentID, A.AgentName
                    ORDER BY SUM(OD.Quantity * OD.UnitAmount) DESC";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvResults.DataSource = dt;

                    FormatCurrencyColumn("Total Purchase Amount");
                    FormatCurrencyColumn("Average Order Value");
                    FormatDateColumn("First Purchase");
                    FormatDateColumn("Last Purchase");
                    lblTitle.Text = "Customer Purchase Summary";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customer purchases: " + ex.Message);
            }
        }

        private void FormatCurrencyColumn(string columnName)
        {
            if (dgvResults.Columns.Contains(columnName))
            {
                dgvResults.Columns[columnName].DefaultCellStyle.Format = "C2";
            }
        }

        private void FormatDateColumn(string columnName)
        {
            if (dgvResults.Columns.Contains(columnName))
            {
                dgvResults.Columns[columnName].DefaultCellStyle.Format = "dd/MM/yyyy";
            }
        }

        private void btnBestItems_Click(object sender, EventArgs e)
        {
            ShowBestItems();
        }

        private void btnItemsByCustomer_Click(object sender, EventArgs e)
        {
            ShowItemsByCustomer();
        }

        private void btnCustomerPurchases_Click(object sender, EventArgs e)
        {
            ShowCustomerPurchases();
        }
    }
}
