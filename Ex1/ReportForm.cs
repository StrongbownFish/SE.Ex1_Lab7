using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ex1
{
    public partial class ReportForm : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
        private DataSet orderData;
        private PrintDocument printDocument;
        private int currentPage = 1;
        private Font titleFont = new Font("Arial", 16, FontStyle.Bold);
        private Font headerFont = new Font("Arial", 12, FontStyle.Bold);
        private Font normalFont = new Font("Arial", 10);
        private int currentY = 0;

        public ReportForm()
        {
            InitializeComponent();
            LoadOrders();
            InitializePrintDocument();
        }

        private void InitializePrintDocument()
        {
            printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;
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
                            COUNT(OD.ItemID) as ItemCount,
                            SUM(OD.Quantity * OD.UnitAmount) as TotalAmount
                        FROM [Order] O
                        INNER JOIN Agent A ON O.AgentID = A.AgentID
                        INNER JOIN OrderDetail OD ON O.OrderID = OD.OrderID
                        GROUP BY O.OrderID, O.OrderDate, A.AgentName
                        ORDER BY O.OrderDate DESC";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvOrders.DataSource = dt;
                    FormatGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading orders: " + ex.Message);
                }
            }
        }

        private void FormatGrid()
        {
            dgvOrders.Columns["OrderID"].HeaderText = "Order ID";
            dgvOrders.Columns["OrderDate"].HeaderText = "Order Date";
            dgvOrders.Columns["AgentName"].HeaderText = "Agent Name";
            dgvOrders.Columns["ItemCount"].HeaderText = "Items";
            dgvOrders.Columns["TotalAmount"].HeaderText = "Total Amount";
            dgvOrders.Columns["TotalAmount"].DefaultCellStyle.Format = "C2";
        }

        private void GetOrderData(int orderId)
        {
            orderData = new DataSet();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    // Get Order Header
                    string headerQuery = @"
                        SELECT 
                            O.OrderID,
                            O.OrderDate,
                            A.AgentName,
                            A.Address
                        FROM [Order] O
                        INNER JOIN Agent A ON O.AgentID = A.AgentID
                        WHERE O.OrderID = @OrderID";

                    SqlDataAdapter headerAdapter = new SqlDataAdapter(headerQuery, conn);
                    headerAdapter.SelectCommand.Parameters.AddWithValue("@OrderID", orderId);
                    headerAdapter.Fill(orderData, "OrderHeader");

                    // Get Order Details
                    string detailsQuery = @"
                        SELECT 
                            I.ItemName,
                            OD.Quantity,
                            OD.UnitAmount,
                            (OD.Quantity * OD.UnitAmount) as TotalAmount
                        FROM OrderDetail OD
                        INNER JOIN Item I ON OD.ItemID = I.ItemID
                        WHERE OD.OrderID = @OrderID";

                    SqlDataAdapter detailsAdapter = new SqlDataAdapter(detailsQuery, conn);
                    detailsAdapter.SelectCommand.Parameters.AddWithValue("@OrderID", orderId);
                    detailsAdapter.Fill(orderData, "OrderDetails");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error getting order data: " + ex.Message);
                }
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            int leftMargin = e.MarginBounds.Left;
            int topMargin = e.MarginBounds.Top;
            int width = e.MarginBounds.Width;
            currentY = topMargin;

            // Reset for new page
            if (currentPage == 1)
            {
                // Print Header
                DataRow headerRow = orderData.Tables["OrderHeader"].Rows[0];

                // Title
                g.DrawString("ORDER DETAILS", titleFont, Brushes.Black,
                    leftMargin + (width - g.MeasureString("ORDER DETAILS", titleFont).Width) / 2, currentY);
                currentY += 30;

                // Order Information
                g.DrawString($"Order ID: {headerRow["OrderID"]}", headerFont, Brushes.Black, leftMargin, currentY);
                currentY += 20;
                g.DrawString($"Date: {Convert.ToDateTime(headerRow["OrderDate"]).ToShortDateString()}",
                    headerFont, Brushes.Black, leftMargin, currentY);
                currentY += 20;
                g.DrawString($"Agent: {headerRow["AgentName"]}", headerFont, Brushes.Black, leftMargin, currentY);
                currentY += 20;
                g.DrawString($"Address: {headerRow["Address"]}", headerFont, Brushes.Black, leftMargin, currentY);
                currentY += 40;

                // Column Headers
                string[] headers = { "Product", "Quantity", "Unit Price", "Total" };
                int[] columnWidths = { width - 300, 100, 100, 100 };
                int x = leftMargin;

                for (int i = 0; i < headers.Length; i++)
                {
                    g.DrawString(headers[i], headerFont, Brushes.Black, x, currentY);
                    x += columnWidths[i];
                }
                currentY += 30;

                // Draw a line under headers
                g.DrawLine(Pens.Black, leftMargin, currentY - 5, leftMargin + width, currentY - 5);
            }

            // Print Details
            decimal total = 0;
            foreach (DataRow row in orderData.Tables["OrderDetails"].Rows)
            {
                if (currentY + 20 > e.MarginBounds.Bottom)
                {
                    e.HasMorePages = true;
                    currentPage++;
                    return;
                }

                int x = leftMargin;
                g.DrawString(row["ItemName"].ToString(), normalFont, Brushes.Black, x, currentY);
                x += width - 300;
                g.DrawString(row["Quantity"].ToString(), normalFont, Brushes.Black, x, currentY);
                x += 100;
                g.DrawString(Convert.ToDecimal(row["UnitAmount"]).ToString("C2"),
                    normalFont, Brushes.Black, x, currentY);
                x += 100;
                decimal itemTotal = Convert.ToDecimal(row["TotalAmount"]);
                g.DrawString(itemTotal.ToString("C2"), normalFont, Brushes.Black, x, currentY);
                total += itemTotal;
                currentY += 20;
            }

            // Print Total
            currentY += 20;
            g.DrawLine(Pens.Black, leftMargin, currentY - 5, leftMargin + width, currentY - 5);
            g.DrawString($"Total Amount: {total:C2}", headerFont, Brushes.Black,
                leftMargin + width - 200, currentY);

            currentPage = 1; // Reset for next print
            e.HasMorePages = false;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (dgvOrders.CurrentRow == null)
            {
                MessageBox.Show("Please select an order to print.");
                return;
            }

            int orderId = Convert.ToInt32(dgvOrders.CurrentRow.Cells["OrderID"].Value);
            GetOrderData(orderId);

            if (orderData?.Tables["OrderHeader"]?.Rows.Count > 0)
            {
                using (PrintPreviewDialog previewDialog = new PrintPreviewDialog())
                {
                    previewDialog.Document = printDocument;
                    previewDialog.WindowState = FormWindowState.Maximized;
                    previewDialog.ShowDialog();
                }
            }
        }

        private void btnExportPDF_Click(object sender, EventArgs e)
        {
            // If you want PDF export, you can use a third-party library like iTextSharp
            MessageBox.Show("PDF export feature can be implemented using iTextSharp or other PDF libraries.");
        }
    }
}
