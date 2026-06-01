using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace bank_management_system
{
    public partial class UserManagementForm : Form
    {
        public UserManagementForm()
        {
            InitializeComponent();
            this.Load += new EventHandler(UserManagementForm_Load);
            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
            this.btnAddUser.Click += new EventHandler(btnAddUser_Click);
            this.btnEditUser.Click += new EventHandler(btnEditUser_Click);
            this.btnDeleteUser.Click += new EventHandler(btnDeleteUser_Click);
            this.btnSearch.Click += new EventHandler(btnSearch_Click);
        }

        private void UserManagementForm_Load(object sender, EventArgs e)
        {
            LoadUsers("");
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            LoadUsers("");
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadUsers(txtSearch.Text.Trim());
        }

        private void LoadUsers(string filter)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["BankDbConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT UserID, Username, Email, FullName, Phone, Role, IsActive 
                                     FROM Users 
                                     WHERE (@Filter = '' OR FullName LIKE '%' + @Filter + '%' OR Username LIKE '%' + @Filter + '%' OR Email LIKE '%' + @Filter + '%')
                                     ORDER BY UserID ASC";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@Filter", filter);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvUsers.DataSource = dt;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading users: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            using (UserEditDialog dialog = new UserEditDialog())
            {
                dialog.IsEditMode = false;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    LoadUsers("");
                }
            }
        }

        private void btnEditUser_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count > 0)
            {
                var row = dgvUsers.SelectedRows[0];
                int userId = Convert.ToInt32(row.Cells["UserID"].Value);
                string username = row.Cells["Username"].Value?.ToString();
                string email = row.Cells["Email"].Value?.ToString();
                string fullName = row.Cells["FullName"].Value?.ToString();
                string phone = row.Cells["Phone"].Value?.ToString();
                string role = row.Cells["Role"].Value?.ToString();
                bool isActive = Convert.ToBoolean(row.Cells["IsActive"].Value);

                using (UserEditDialog dialog = new UserEditDialog())
                {
                    dialog.IsEditMode = true;
                    dialog.EditedUserID = userId;
                    dialog.LoadUserData(username, email, fullName, phone, role, isActive);

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        LoadUsers("");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a user to edit.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count > 0)
            {
                var userId = dgvUsers.SelectedRows[0].Cells["UserID"].Value;
                if (userId != null)
                {
                    DialogResult result = MessageBox.Show($"Are you sure you want to deactivate User ID {userId}? This will revoke their login access.", "Confirm Deactivation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            string connectionString = ConfigurationManager.ConnectionStrings["BankDbConnection"].ConnectionString;
                            using (SqlConnection conn = new SqlConnection(connectionString))
                            {
                                conn.Open();
                                // Soft delete to prevent breaking Foreign Keys
                                using (SqlCommand cmd = new SqlCommand("UPDATE Users SET IsActive = 0 WHERE UserID = @UserID", conn))
                                {
                                    cmd.Parameters.AddWithValue("@UserID", userId);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            LoadUsers("");
                            MessageBox.Show("User successfully deactivated.", "Deactivated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error deactivating user: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a user to deactivate.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
