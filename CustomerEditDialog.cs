using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace bank_management_system
{
    public partial class CustomerEditDialog : Form
    {
        public bool IsEditMode { get; set; } = false;
        public int CustomerId { get; set; } = 0;

        public CustomerEditDialog()
        {
            InitializeComponent();
        }

        public void LoadCustomerData(string username, string email, string fullName, string phone)
        {
            txtUsername.Text = username;
            txtEmail.Text = email;
            txtFullName.Text = fullName;
            txtPhone.Text = phone;
            this.Text = "Edit Customer";
            lblTitle.Text = "Edit Customer";
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
                string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Username, Email, and Full Name are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsEditMode && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Password is required for new customers.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        string query = @"UPDATE Users SET Username = @Username, Email = @Email, FullName = @FullName, Phone = @Phone ";
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
                            cmd.Parameters.AddWithValue("@UserID", CustomerId);

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
                            VALUES (@Username, @Email, @PasswordHash, @FullName, @Phone, 'Customer', 1);
                            
                            DECLARE @NewUserID INT = SCOPE_IDENTITY();
                            
                            INSERT INTO Customers (UserID, CustomerCode, FullName, Phone, Email, Status, CreatedAt)
                            VALUES (@NewUserID, 'CUST-' + CAST(@NewUserID AS NVARCHAR), @FullName, @Phone, @Email, 'Active', GETDATE());
                        ";
                        
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                            cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                            cmd.Parameters.AddWithValue("@PasswordHash", HashPassword(txtPassword.Text));
                            cmd.Parameters.AddWithValue("@FullName", txtFullName.Text);
                            cmd.Parameters.AddWithValue("@Phone", txtPhone.Text);
                            
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving customer: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
