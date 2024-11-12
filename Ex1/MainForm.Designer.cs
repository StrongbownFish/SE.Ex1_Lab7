using System.Windows.Forms;

namespace Ex1
{
    partial class MainForm
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
            this.panelMenu = new System.Windows.Forms.Panel();
            this.btnProducts = new System.Windows.Forms.Button();
            this.btnAgents = new System.Windows.Forms.Button();
            this.btnOrders = new System.Windows.Forms.Button();
            this.btnFilter = new System.Windows.Forms.Button();

            // panelTop
            this.panelTop.BackColor = System.Drawing.Color.RoyalBlue;
            this.panelTop.Controls.Add(this.lblTitle);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Height = 60;

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(12, 15);
            this.lblTitle.Text = "Order Management System";

            // panelMenu
            this.panelMenu.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenu.Width = 220;
            this.panelMenu.Padding = new System.Windows.Forms.Padding(10);

            // Style for all buttons
            System.Drawing.Font buttonFont = new System.Drawing.Font("Segoe UI", 10.75F, System.Drawing.FontStyle.Regular);
            System.Drawing.Size buttonSize = new System.Drawing.Size(200, 45);
            System.Drawing.Color buttonBackColor = System.Drawing.Color.RoyalBlue;
            System.Drawing.Color buttonForeColor = System.Drawing.Color.White;

            // btnProducts
            this.btnProducts.Text = "Products Management";
            this.btnProducts.Font = buttonFont;
            this.btnProducts.Size = buttonSize;
            this.btnProducts.Location = new System.Drawing.Point(10, 20);
            this.btnProducts.BackColor = buttonBackColor;
            this.btnProducts.ForeColor = buttonForeColor;
            this.btnProducts.FlatStyle = FlatStyle.Flat;
            this.btnProducts.Cursor = Cursors.Hand;
            this.btnProducts.Click += new System.EventHandler(this.btnProducts_Click);

            // btnAgents
            this.btnAgents.Text = "Agents Management";
            this.btnAgents.Font = buttonFont;
            this.btnAgents.Size = buttonSize;
            this.btnAgents.Location = new System.Drawing.Point(10, 80);
            this.btnAgents.BackColor = buttonBackColor;
            this.btnAgents.ForeColor = buttonForeColor;
            this.btnAgents.FlatStyle = FlatStyle.Flat;
            this.btnAgents.Cursor = Cursors.Hand;
            this.btnAgents.Click += new System.EventHandler(this.btnAgents_Click);

            // btnOrders
            this.btnOrders.Text = "Orders Management";
            this.btnOrders.Font = buttonFont;
            this.btnOrders.Size = buttonSize;
            this.btnOrders.Location = new System.Drawing.Point(10, 140);
            this.btnOrders.BackColor = buttonBackColor;
            this.btnOrders.ForeColor = buttonForeColor;
            this.btnOrders.FlatStyle = FlatStyle.Flat;
            this.btnOrders.Cursor = Cursors.Hand;
            this.btnOrders.Click += new System.EventHandler(this.btnOrders_Click);
            // btnFilter
            this.btnFilter.Text = "Reports & Analysis";
            this.btnFilter.Font = buttonFont;
            this.btnFilter.Size = buttonSize;
            this.btnFilter.Location = new System.Drawing.Point(10, 200);  // Position below Orders button
            this.btnFilter.BackColor = buttonBackColor;
            this.btnFilter.ForeColor = buttonForeColor;
            this.btnFilter.FlatStyle = FlatStyle.Flat;
            this.btnFilter.Cursor = Cursors.Hand;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // Add buttons to menu panel
            this.panelMenu.Controls.AddRange(new Control[] {
                btnProducts,
                btnAgents,
                btnOrders,
                btnFilter
            });

            // Add welcome label
            Label lblWelcome = new Label();
            lblWelcome.AutoSize = true;
            lblWelcome.Font = new System.Drawing.Font("Segoe UI", 14F);
            lblWelcome.Location = new System.Drawing.Point(250, 100);
            lblWelcome.Text = "Welcome to Order Management System";

            // MainForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.Controls.AddRange(new Control[] {
                panelTop,
                panelMenu,
                lblWelcome
            });
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(816, 539);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Order Management System";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = System.Drawing.Color.White;

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Button btnProducts;
        private System.Windows.Forms.Button btnAgents;
        private System.Windows.Forms.Button btnOrders;
        private System.Windows.Forms.Button btnFilter;
    }
}