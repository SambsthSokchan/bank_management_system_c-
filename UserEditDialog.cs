using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace bank_management_system
{
    public partial class UserEditDialog : Form
    {
        public bool IsEditMode { get; set; } = false;
        public int EditedUserID { get; set; } = 0;

        public UserEditDialog()
        {
            InitializeComponent();
            this.Load += new EventHandler(UserEditDialog_Load);
        }

        private void UserEditDialog_Load(object sender, EventArgs e)
        {
            if (!IsEditMode)
            {
                cmbRole.SelectedIndex = 0;
            }
        }

        public void LoadUserData(string username, string email, string fullName, string phone, string role, bool isActive)
        {
            txtUsername.Text = username;
            txtEmail.Text = email;
            txtFullName.Text = fullName;
            txtPhone.Text = phone;
            cmbRole.SelectedItem = role;
            chkIsActive.Checked = isActive;
            
            this.Text = "Edit User";
            lblTitle.Text = "Edit User";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || 
                string.IsNullOrWhiteSpace(txtEmail.Text) || 
                string.IsNullOrWhiteSpace(txtFullName.Text) ||
                cmbRole.SelectedItem == null)
            {
                MessageBox.Show("Username, Email, Full Name, and Role are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsEditMode && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Password is required for new users.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["BankDbConnection"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    if (IsEditMode)
                    {
                        string query = @"UPDATE Users SET Username = @Username, Email = @Email, FullName = @FullName, 
                                         Phone = @Phone, Role = @Role, IsActive = @IsActive ";
                        if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                        {
                            query += ", PasswordHash = @PasswordHash ";
                        }
                        query += "WHERE UserID = @UserID";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                            cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                            cmd.Parameters.AddWithValue("@FullName", txtFullName.Text);
                            cmd.Parameters.AddWithValue("@Phone", txtPhone.Text);
                            cmd.Parameters.AddWithValue("@Role", cmbRole.SelectedItem.ToString());
                            cmd.Parameters.AddWithValue("@IsActive", chkIsActive.Checked ? 1 : 0);
                            cmd.Parameters.AddWithValue("@UserID", EditedUserID);

                            if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                            {
                                cmd.Parameters.AddWithValue("@PasswordHash", HashPassword(txtPassword.Text));
                            }
                            
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string query = @"
                            INSERT INTO Users (Username, Email, PasswordHash, FullName, Phone, Role, IsActive) 
                            VALUES (@Username, @Email, @PasswordHash, @FullName, @Phone, @Role, @IsActive);
                        ";
                        
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                            cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                            cmd.Parameters.AddWithValue("@PasswordHash", HashPassword(txtPassword.Text));
                            cmd.Parameters.AddWithValue("@FullName", txtFullName.Text);
                            cmd.Parameters.AddWithValue("@Phone", txtPhone.Text);
                            cmd.Parameters.AddWithValue("@Role", cmbRole.SelectedItem.ToString());
                            cmd.Parameters.AddWithValue("@IsActive", chkIsActive.Checked ? 1 : 0);
                            
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving user: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private void UserEditDialog_Load_1(object sender, EventArgs e)
        {

        }
    }
}
