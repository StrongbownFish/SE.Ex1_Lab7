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
                    string query = "SELECT AgentID, AgentName, Address FROM Agent ORDER BY AgentID";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvAgents.DataSource = dt;

                    // Format columns
                    dgvAgents.Columns["AgentID"].HeaderText = "Agent ID";
                    dgvAgents.Columns["AgentName"].HeaderText = "Agent Name";
                    dgvAgents.Columns["Address"].HeaderText = "Address";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading agents: " + ex.Message);
                }
            }
        }
        private int GetNextAgentId()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ISNULL(MAX(AgentID), 0) + 1 FROM Agent";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    return (int)cmd.ExecuteScalar();
                }
                catch (Exception)
                {
                    return 1; // Return 1 if there's an error or table is empty
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(txtAgentName.Text))
            {
                MessageBox.Show("Please enter an agent name.");
                txtAgentName.Focus();
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query;
                    SqlCommand cmd;

                    if (selectedAgentId.HasValue)
                    {
                        // Update existing agent
                        query = @"UPDATE Agent 
                             SET AgentName = @name, Address = @address 
                             WHERE AgentID = @id";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", selectedAgentId.Value);
                    }
                    else
                    {
                        // Insert new agent with next available ID
                        int nextId = GetNextAgentId();
                        query = @"INSERT INTO Agent (AgentID, AgentName, Address) 
                             VALUES (@id, @name, @address)";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", nextId);
                    }

                    cmd.Parameters.AddWithValue("@name", txtAgentName.Text.Trim());
                    cmd.Parameters.AddWithValue("@address", txtAddress.Text.Trim());

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

            // Check if agent has any orders before deleting
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string checkQuery = "SELECT COUNT(*) FROM [Order] WHERE AgentID = @id";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@id", selectedAgentId.Value);
                    int orderCount = (int)checkCmd.ExecuteScalar();

                    if (orderCount > 0)
                    {
                        MessageBox.Show("Cannot delete this agent because they have existing orders.");
                        return;
                    }

                    if (MessageBox.Show("Are you sure you want to delete this agent?", "Confirm Delete",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        string deleteQuery = "DELETE FROM Agent WHERE AgentID = @id";
                        SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn);
                        deleteCmd.Parameters.AddWithValue("@id", selectedAgentId.Value);
                        deleteCmd.ExecuteNonQuery();

                        MessageBox.Show("Agent deleted successfully!");
                        ClearForm();
                        LoadAgents();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting agent: " + ex.Message);
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
            txtAgentName.Focus();
        }
    }
}
