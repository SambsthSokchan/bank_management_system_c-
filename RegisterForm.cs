using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;

namespace bank_management_system
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string phoneNumber = txtPhoneNumber.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            // Basic validation
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || 
                string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || 
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Please fill in all required fields (Phone Number is optional).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Register user in database
            string connectionString = ConfigurationManager.ConnectionStrings["BankDbConnection"].ConnectionString;
            
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if email or username already exists
                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email OR Username = @Username";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", email);
                        checkCmd.Parameters.AddWithValue("@Username", username); 
                        
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show("An account with this email or username already exists.", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Insert new user and get the UserID
                    string insertQuery = @"INSERT INTO Users (Username, Email, PasswordHash, FullName, Phone, Role, IsActive) 
                                           OUTPUT INSERTED.UserID
                                           VALUES (@Username, @Email, @PasswordHash, @FullName, @Phone, 'Customer', 1)";
                    
                    int newUserId = 0;
                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@PasswordHash", HashPassword(password));
                        cmd.Parameters.AddWithValue("@FullName", firstName + " " + lastName);
                        
                        if (string.IsNullOrWhiteSpace(phoneNumber))
                            cmd.Parameters.AddWithValue("@Phone", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@Phone", phoneNumber);

                        newUserId = (int)cmd.ExecuteScalar();
                    }

                    // Insert into Customers table
                    if (newUserId > 0)
                    {
                        string customerQuery = @"INSERT INTO Customers (UserID, CustomerCode, FullName, Phone, Email, Status)
                                                 VALUES (@UserID, @CustomerCode, @FullName, @Phone, @Email, 'Active')";
                        using (SqlCommand custCmd = new SqlCommand(customerQuery, conn))
                        {
                            custCmd.Parameters.AddWithValue("@UserID", newUserId);
                            custCmd.Parameters.AddWithValue("@CustomerCode", "CUST-" + newUserId.ToString());
                            custCmd.Parameters.AddWithValue("@FullName", firstName + " " + lastName);
                            
                            if (string.IsNullOrWhiteSpace(phoneNumber))
                                custCmd.Parameters.AddWithValue("@Phone", DBNull.Value);
                            else
                                custCmd.Parameters.AddWithValue("@Phone", phoneNumber);
                                
                            custCmd.Parameters.AddWithValue("@Email", email);
                            custCmd.ExecuteNonQuery();
                        }
                        
                        // Also auto-create a default Savings account for them!
                        // We need the CustomerID which we can get usingSCOPE_IDENTITY() or querying
                        string accountQuery = @"
                            DECLARE @NewCustID INT;
                            SELECT @NewCustID = CustomerID FROM Customers WHERE UserID = @UserID;
                            INSERT INTO Accounts (CustomerID, AccountNumber, AccountType, Balance, Status)
                            VALUES (@NewCustID, 'ACC-' + RIGHT('0000000000' + CAST(ABS(CHECKSUM(NEWID())) AS VARCHAR), 10), 'Savings', 0.00, 'Active')";
                        using (SqlCommand accCmd = new SqlCommand(accountQuery, conn))
                        {
                            accCmd.Parameters.AddWithValue("@UserID", newUserId);
                            accCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Registration successful! You can now log in.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            // Go back to login form
            this.Hide();
            LoginForm loginForm = new LoginForm();
            loginForm.FormClosed += (s, args) => this.Close();
            loginForm.Show();
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private void linkLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            LoginForm loginForm = new LoginForm();
            loginForm.FormClosed += (s, args) => this.Close();
            loginForm.Show();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            LoginForm login = new LoginForm();
            login.FormClosed += (s, args) => this.Close();
            login.Show();
            this.Hide();
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {

        }
    }
}
