using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace bank_management_system
{
    public partial class CustomerForm : Form
    {
        public CustomerForm()
        {
            InitializeComponent();
            this.Load += new EventHandler(CustomerForm_Load);
            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
            this.btnAddCustomer.Click += new EventHandler(btnAddCustomer_Click);
            this.btnEditCustomer.Click += new EventHandler(btnEditCustomer_Click);
            this.btnDeleteCustomer.Click += new EventHandler(btnDeleteCustomer_Click);
            this.btnSearch.Click += new EventHandler(btnSearch_Click);
        }

        private void CustomerForm_Load(object sender, EventArgs e)
        {
            LoadCustomers("");
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            LoadCustomers("");
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadCustomers(txtSearch.Text.Trim());
        }

        private void LoadCustomers(string filter)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["BankDbConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Load all users who have the role 'Customer'
                    string query = @"SELECT UserID, Username, Email, FullName, Phone, IsActive 
                                     FROM Users 
                                     WHERE Role = 'Customer' 
                                     AND (@Filter = '' OR FullName LIKE '%' + @Filter + '%' OR Username LIKE '%' + @Filter + '%')";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@Filter", filter);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvCustomers.DataSource = dt;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading customers: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            using (CustomerEditDialog dialog = new CustomerEditDialog())
            {
                dialog.IsEditMode = false;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    LoadCustomers("");
                }
            }
        }

        private void btnEditCustomer_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count > 0)
            {
                var row = dgvCustomers.SelectedRows[0];
                int userId = Convert.ToInt32(row.Cells["UserID"].Value);
                string username = row.Cells["Username"].Value?.ToString();
                string email = row.Cells["Email"].Value?.ToString();
                string fullName = row.Cells["FullName"].Value?.ToString();
                string phone = row.Cells["Phone"].Value?.ToString();

                using (CustomerEditDialog dialog = new CustomerEditDialog())
                {
                    dialog.IsEditMode = true;
                    dialog.CustomerId = userId;
                    dialog.LoadCustomerData(username, email, fullName, phone);

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        LoadCustomers("");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a customer to edit.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDeleteCustomer_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count > 0)
            {
                var userId = dgvCustomers.SelectedRows[0].Cells["UserID"].Value;
                if (userId != null)
                {
                    DialogResult result = MessageBox.Show($"Are you sure you want to deactivate Customer ID {userId}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            string connectionString = ConfigurationManager.ConnectionStrings["BankDbConnection"].ConnectionString;
                            using (SqlConnection conn = new SqlConnection(connectionString))
                            {
                                conn.Open();
                                using (SqlCommand cmd = new SqlCommand("UPDATE Users SET IsActive = 0 WHERE UserID = @UserID", conn))
                                {
                                    cmd.Parameters.AddWithValue("@UserID", userId);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            LoadCustomers(""); // Refresh grid
                            MessageBox.Show("Customer successfully deactivated.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error deleting customer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a customer to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvCustomers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
