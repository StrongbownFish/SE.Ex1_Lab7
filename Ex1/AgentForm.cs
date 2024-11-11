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
    public partial class AgentForm : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DevConn"].ConnectionString;
        private int? selectedAgentId = null;

        public AgentForm()
        {
            InitializeComponent();
            LoadAgents();
        }

        private void LoadAgents()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT AgentID, AgentName, Address FROM Agent";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvAgents.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading agents: " + ex.Message);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query;
                    if (selectedAgentId.HasValue)
                    {
                        query = @"UPDATE Agent 
                                 SET AgentName = @name, Address = @address 
                                 WHERE AgentID = @id";
                    }
                    else
                    {
                        query = @"INSERT INTO Agent (AgentName, Address) 
                                 VALUES (@name, @address)";
                    }

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", txtAgentName.Text);
                    cmd.Parameters.AddWithValue("@address", txtAddress.Text);
                    if (selectedAgentId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@id", selectedAgentId.Value);
                    }

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Agent saved successfully!");
                    ClearForm();
                    LoadAgents();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving agent: " + ex.Message);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!selectedAgentId.HasValue)
            {
                MessageBox.Show("Please select an agent to delete.");
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this agent?", "Confirm Delete",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = "DELETE FROM Agent WHERE AgentID = @id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", selectedAgentId.Value);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Agent deleted successfully!");
                        ClearForm();
                        LoadAgents();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting agent: " + ex.Message);
                    }
                }
            }
        }

        private void dgvAgents_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedAgentId = Convert.ToInt32(dgvAgents.Rows[e.RowIndex].Cells["AgentID"].Value);
                txtAgentName.Text = dgvAgents.Rows[e.RowIndex].Cells["AgentName"].Value.ToString();
                txtAddress.Text = dgvAgents.Rows[e.RowIndex].Cells["Address"].Value.ToString();
                btnSave.Text = "Update";
                btnDelete.Enabled = true;
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            selectedAgentId = null;
            txtAgentName.Clear();
            txtAddress.Clear();
            btnSave.Text = "Save";
            btnDelete.Enabled = false;
        }
    }
}
