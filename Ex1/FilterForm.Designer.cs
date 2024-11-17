using System.Windows.Forms;

namespace Ex1
{
    partial class FilterForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region
        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnBestItems = new System.Windows.Forms.Button();
            this.btnCustomerPurchases = new System.Windows.Forms.Button();
            this.btnItemsByCustomer = new System.Windows.Forms.Button();
            this.dgvResults = new System.Windows.Forms.DataGridView();
            this.panelTop.SuspendLayout();
            this.panelButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
            this.SuspendLayout();

            // panelTop
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(77)))), ((int)(((byte)(255)))));
            this.panelTop.Controls.Add(this.lblTitle);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 50);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(800, 50);
            this.panelTop.TabIndex = 0;

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(12, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(168, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Data Analysis";

            // panelButtons
            this.panelButtons.Controls.Add(this.btnBestItems);
            this.panelButtons.Controls.Add(this.btnCustomerPurchases);
            this.panelButtons.Controls.Add(this.btnItemsByCustomer);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelButtons.Location = new System.Drawing.Point(0, 0);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Padding = new System.Windows.Forms.Padding(8);
            this.panelButtons.Size = new System.Drawing.Size(800, 50);
            this.panelButtons.TabIndex = 2;

            // btnBestItems
            this.btnBestItems.Location = new System.Drawing.Point(12, 8);
            this.btnBestItems.Name = "btnBestItems";
            this.btnBestItems.Size = new System.Drawing.Size(120, 28);
            this.btnBestItems.TabIndex = 0;
            this.btnBestItems.Text = "Best Items";
            this.btnBestItems.UseVisualStyleBackColor = true;
            this.btnBestItems.Click += new System.EventHandler(this.btnBestItems_Click);

            // btnCustomerPurchases
            this.btnCustomerPurchases.Location = new System.Drawing.Point(142, 8);
            this.btnCustomerPurchases.Name = "btnCustomerPurchases";
            this.btnCustomerPurchases.Size = new System.Drawing.Size(140, 28);
            this.btnCustomerPurchases.TabIndex = 1;
            this.btnCustomerPurchases.Text = "Customer Purchases";
            this.btnCustomerPurchases.UseVisualStyleBackColor = true;
            this.btnCustomerPurchases.Click += new System.EventHandler(this.btnCustomerPurchases_Click);

            // btnItemsByCustomer
            this.btnItemsByCustomer.Location = new System.Drawing.Point(292, 8);
            this.btnItemsByCustomer.Name = "btnItemsByCustomer";
            this.btnItemsByCustomer.Size = new System.Drawing.Size(140, 28);
            this.btnItemsByCustomer.TabIndex = 2;
            this.btnItemsByCustomer.Text = "Items by Customer";
            this.btnItemsByCustomer.UseVisualStyleBackColor = true;
            this.btnItemsByCustomer.Click += new System.EventHandler(this.btnItemsByCustomer_Click);

            // dgvResults
            this.dgvResults.AllowUserToAddRows = false;
            this.dgvResults.AllowUserToDeleteRows = false;
            this.dgvResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvResults.BackgroundColor = System.Drawing.Color.White;
            this.dgvResults.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvResults.Location = new System.Drawing.Point(0, 100);
            this.dgvResults.Name = "dgvResults";
            this.dgvResults.ReadOnly = true;
            this.dgvResults.RowHeadersWidth = 51;
            this.dgvResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvResults.Size = new System.Drawing.Size(800, 500);
            this.dgvResults.TabIndex = 3;

            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.dgvResults);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelButtons);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(816, 639);
            this.Name = "FilterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Data Analysis";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnBestItems;
        private System.Windows.Forms.Button btnCustomerPurchases;
        private System.Windows.Forms.Button btnItemsByCustomer;
        private System.Windows.Forms.DataGridView dgvResults;
    }
}