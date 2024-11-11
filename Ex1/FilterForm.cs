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
            SetupDateRanges();
        }

        private void SetupDateRanges()
        {
            dtpFromDate.Value = DateTime.Now.AddMonths(-1);
            dtpToDate.Value = DateTime.Now;
        }

        private void btnBestItems_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    string query = @"
                        SELECT 
                            I.ItemName as 'Product Name',
                            SUM(OD.Quantity) as 'Total Quantity',
                            COUNT(DISTINCT O.OrderID) as 'Number of Orders',
                            SUM(OD.Quantity * OD.UnitAmount) as 'Total Revenue',
                            AVG(OD.UnitAmount) as 'Average Price'
                        FROM Item I
                        LEFT JOIN OrderDetail OD ON I.ItemID = OD.ItemID
                        LEFT JOIN [Order] O ON OD.OrderID = O.OrderID
                        WHERE O.OrderDate BETWEEN @FromDate AND @ToDate
                        GROUP BY I.ItemID, I.ItemName
                        ORDER BY SUM(OD.Quantity) DESC";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    adapter.SelectCommand.Parameters.AddWithValue("@FromDate", dtpFromDate.Value.Date);
                    adapter.SelectCommand.Parameters.AddWithValue("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1));

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvResults.DataSource = dt;

                    FormatBestItemsGrid();
                    lblTitle.Text = "Best Selling Items";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading best items: " + ex.Message);
                }
            }
        }

        private void btnCustomerPurchases_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    string query = @"
                        SELECT 
                            A.AgentName as 'Customer Name',
                            COUNT(DISTINCT O.OrderID) as 'Total Orders',
                            SUM(OD.Quantity * OD.UnitAmount) as 'Total Amount',
                            MAX(O.OrderDate) as 'Last Order Date',
                            AVG(OD.Quantity * OD.UnitAmount) as 'Average Order Value'
                        FROM Agent A
                        LEFT JOIN [Order] O ON A.AgentID = O.AgentID
                        LEFT JOIN OrderDetail OD ON O.OrderID = OD.OrderID
                        WHERE O.OrderDate BETWEEN @FromDate AND @ToDate
                        GROUP BY A.AgentID, A.AgentName
                        ORDER BY SUM(OD.Quantity * OD.UnitAmount) DESC";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    adapter.SelectCommand.Parameters.AddWithValue("@FromDate", dtpFromDate.Value.Date);
                    adapter.SelectCommand.Parameters.AddWithValue("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1));

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvResults.DataSource = dt;

                    FormatCustomerPurchasesGrid();
                    lblTitle.Text = "Customer Purchase Analysis";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading customer purchases: " + ex.Message);
                }
            }
        }

        private void btnItemsByCustomer_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    string query = @"
                        SELECT 
                            A.AgentName as 'Customer Name',
                            I.ItemName as 'Product Name',
                            SUM(OD.Quantity) as 'Total Quantity',
                            SUM(OD.Quantity * OD.UnitAmount) as 'Total Amount',
                            COUNT(DISTINCT O.OrderID) as 'Number of Orders',
                            MAX(O.OrderDate) as 'Last Purchase Date'
                        FROM Agent A
                        JOIN [Order] O ON A.AgentID = O.AgentID
                        JOIN OrderDetail OD ON O.OrderID = OD.OrderID
                        JOIN Item I ON OD.ItemID = I.ItemID
                        WHERE O.OrderDate BETWEEN @FromDate AND @ToDate
                        GROUP BY A.AgentID, A.AgentName, I.ItemID, I.ItemName
                        ORDER BY A.AgentName, SUM(OD.Quantity) DESC";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    adapter.SelectCommand.Parameters.AddWithValue("@FromDate", dtpFromDate.Value.Date);
                    adapter.SelectCommand.Parameters.AddWithValue("@ToDate", dtpToDate.Value.Date.AddDays(1).AddSeconds(-1));

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvResults.DataSource = dt;

                    FormatItemsByCustomerGrid();
                    lblTitle.Text = "Items Purchased by Customer";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading items by customer: " + ex.Message);
                }
            }
        }

        private void FormatBestItemsGrid()
        {
            foreach (DataGridViewColumn col in dgvResults.Columns)
            {
                switch (col.Name)
                {
                    case "Total Revenue":
                    case "Average Price":
                        col.DefaultCellStyle.Format = "C2";
                        break;
                    case "Total Quantity":
                    case "Number of Orders":
                        col.DefaultCellStyle.Format = "N0";
                        break;
                }
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        private void FormatCustomerPurchasesGrid()
        {
            foreach (DataGridViewColumn col in dgvResults.Columns)
            {
                switch (col.Name)
                {
                    case "Total Amount":
                    case "Average Order Value":
                        col.DefaultCellStyle.Format = "C2";
                        break;
                    case "Total Orders":
                        col.DefaultCellStyle.Format = "N0";
                        break;
                    case "Last Order Date":
                        col.DefaultCellStyle.Format = "MM/dd/yyyy";
                        break;
                }
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        private void FormatItemsByCustomerGrid()
        {
            foreach (DataGridViewColumn col in dgvResults.Columns)
            {
                switch (col.Name)
                {
                    case "Total Amount":
                        col.DefaultCellStyle.Format = "C2";
                        break;
                    case "Total Quantity":
                    case "Number of Orders":
                        col.DefaultCellStyle.Format = "N0";
                        break;
                    case "Last Purchase Date":
                        col.DefaultCellStyle.Format = "MM/dd/yyyy";
                        break;
                }
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }
    }
}
