using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ex1
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            ProductForm productForm = new ProductForm();
            productForm.ShowDialog();
        }

        private void btnAgents_Click(object sender, EventArgs e)
        {
            AgentForm agentForm = new AgentForm();
            agentForm.ShowDialog();
        }

        private void btnOrders_Click(object sender, EventArgs e)
        {
            OrderForm orderForm = new OrderForm();
            orderForm.ShowDialog();
        }
        private void btnFilter_Click(object sender, EventArgs e)
        {
            FilterForm filterForm = new FilterForm();
            filterForm.ShowDialog();
        }
        private void btnOrderDetail_Click(object sender, EventArgs e)
        {
            OrderDetail orderDetailForm = new OrderDetail();
            orderDetailForm.ShowDialog();
        }
    }
}
