using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ex1
{
    public partial class PrintOrderForm : Form
    {
        private DataTable orderData;
        private PrintDocument printDocument;

        public PrintOrderForm(DataTable data)
        {
            InitializeComponent();
            orderData = data;
            SetupPrintDocument();
        }

        private void SetupPrintDocument()
        {
            printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;

            // Set up print preview control
            printPreviewControl.Document = printDocument;
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            Font headerFont = new Font("Arial", 12, FontStyle.Bold);
            Font normalFont = new Font("Arial", 10);
            int yPos = 50;

            // Print header
            DataRow firstRow = orderData.Rows[0];
            g.DrawString("ORDER RECEIPT", titleFont, Brushes.Black, 300, yPos);
            yPos += 40;

            // Order information
            g.DrawString($"Order ID: {firstRow["OrderID"]}", headerFont, Brushes.Black, 50, yPos);
            yPos += 25;
            g.DrawString($"Date: {Convert.ToDateTime(firstRow["OrderDate"]).ToString("dd/MM/yyyy")}",
                headerFont, Brushes.Black, 50, yPos);
            yPos += 25;
            g.DrawString($"Agent: {firstRow["AgentName"]}", headerFont, Brushes.Black, 50, yPos);
            yPos += 25;
            g.DrawString($"Address: {firstRow["Address"]}", headerFont, Brushes.Black, 50, yPos);
            yPos += 40;

            // Column headers
            g.DrawString("Item", headerFont, Brushes.Black, 50, yPos);
            g.DrawString("Quantity", headerFont, Brushes.Black, 300, yPos);
            g.DrawString("Unit Price", headerFont, Brushes.Black, 400, yPos);
            g.DrawString("Total", headerFont, Brushes.Black, 500, yPos);
            yPos += 25;

            // Draw a line
            g.DrawLine(Pens.Black, 50, yPos, 580, yPos);
            yPos += 10;

            // Print items
            decimal grandTotal = 0;
            foreach (DataRow row in orderData.Rows)
            {
                g.DrawString(row["ItemName"].ToString(), normalFont, Brushes.Black, 50, yPos);
                g.DrawString(row["Quantity"].ToString(), normalFont, Brushes.Black, 300, yPos);
                g.DrawString(Convert.ToDecimal(row["UnitAmount"]).ToString("C2"),
                    normalFont, Brushes.Black, 400, yPos);
                decimal total = Convert.ToDecimal(row["TotalAmount"]);
                g.DrawString(total.ToString("C2"), normalFont, Brushes.Black, 500, yPos);
                grandTotal += total;
                yPos += 20;
            }

            // Draw a line before total
            yPos += 10;
            g.DrawLine(Pens.Black, 50, yPos, 580, yPos);
            yPos += 20;

            // Print grand total
            g.DrawString("Grand Total:", headerFont, Brushes.Black, 400, yPos);
            g.DrawString(grandTotal.ToString("C2"), headerFont, Brushes.Black, 500, yPos);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            using (PrintDialog printDialog = new PrintDialog())
            {
                printDialog.Document = printDocument;
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDocument.Print();
                }
            }
        }
    }
}
