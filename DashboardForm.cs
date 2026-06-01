using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace bank_management_system
{
    public partial class DashboardForm : Form
    {
        private string currentUserRole;
        private string currentUsername;

        public DashboardForm(string role = "Admin", string username = "")
        {
            InitializeComponent();
            currentUserRole = role;
            currentUsername = username;
            this.Load += new EventHandler(DashboardForm_Load);
        }

        private void DashboardForm_Load(object sender, EventArgs e)
        {
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["BankDbConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    if (currentUserRole == "Customer")
                    {
                        lblDashboardTitle.Text = "My Banking Overview";
                        lblTotalCustomers.Text = "Account Number";
                        lblTotalAccounts.Text = "Available Balance";
                        lblTotalEmployees.Text = "Recent Activity";
                        lblTotalTransactions.Text = "Account Status";
                        lblRecentTransactions.Text = "Recent Activity";

                        // Reduce font size so the long account number fits in the card
                        lblTotalCustomersVal.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
                        
                        // 1. Account Number
                        try {
                            using (SqlCommand cmd = new SqlCommand("SELECT AccountNumber FROM Accounts WHERE CustomerID IN (SELECT CustomerID FROM Customers WHERE UserID IN (SELECT UserID FROM Users WHERE Username = @User))", conn)) {
                                cmd.Parameters.AddWithValue("@User", currentUsername);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        string accNum = reader.GetString(0);
                                        if (reader.Read())
                                            lblTotalCustomersVal.Text = "Multiple"; // If they have more than 1
                                        else
                                            lblTotalCustomersVal.Text = accNum;
                                    }
                                    else
                                    {
                                        lblTotalCustomersVal.Text = "None";
                                    }
                                }
                            }
                        } catch { lblTotalCustomersVal.Text = "None"; }

                        // 2. Total Balance
                        try {
                            using (SqlCommand cmd = new SqlCommand("SELECT SUM(Balance) FROM Accounts WHERE CustomerID IN (SELECT CustomerID FROM Customers WHERE UserID IN (SELECT UserID FROM Users WHERE Username = @User))", conn)) {
                                cmd.Parameters.AddWithValue("@User", currentUsername);
                                object result = cmd.ExecuteScalar();
                                lblTotalAccountsVal.Text = (result != DBNull.Value) ? "$" + Convert.ToDecimal(result).ToString("N2") : "$0.00";
                            }
                        } catch { lblTotalAccountsVal.Text = "$0.00"; }

                        // 3. Recent Transactions Count
                        try {
                            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Transactions WHERE AccountID IN (SELECT AccountID FROM Accounts WHERE CustomerID IN (SELECT CustomerID FROM Customers WHERE UserID IN (SELECT UserID FROM Users WHERE Username = @User)))", conn)) {
                                cmd.Parameters.AddWithValue("@User", currentUsername);
                                object result = cmd.ExecuteScalar();
                                lblTotalEmployeesVal.Text = result != DBNull.Value ? result.ToString() : "0";
                            }
                        } catch { lblTotalEmployeesVal.Text = "0"; }

                        // 4. My Profile Status
                        lblTotalTransactionsVal.Text = "Active";

                        // DataGridView
                        try {
                            string query = @"SELECT TOP 5 TransactionDate AS 'Date', TransactionType AS 'Type', 
                                             CASE WHEN TransactionType = 'Withdrawal' THEN -Amount ELSE Amount END AS Amount, 
                                             Description FROM Transactions 
                                             WHERE AccountID IN (SELECT AccountID FROM Accounts WHERE CustomerID IN (SELECT CustomerID FROM Customers WHERE UserID IN (SELECT UserID FROM Users WHERE Username = @User))) 
                                             ORDER BY TransactionDate DESC";
                            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn)) {
                                adapter.SelectCommand.Parameters.AddWithValue("@User", currentUsername);
                                DataTable dt = new DataTable();
                                adapter.Fill(dt);
                                dgvRecentTransactions.DataSource = dt;
                            }
                        } catch { }
                    }
                    else
                    {
                        // 1. Total Customers
                        using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Role = 'Customer'", conn))
                        {
                            object result = cmd.ExecuteScalar();
                            lblTotalCustomersVal.Text = result != DBNull.Value ? result.ToString() : "0";
                        }

                        // 2. Total Accounts
                        try
                        {
                            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Accounts", conn))
                            {
                                object result = cmd.ExecuteScalar();
                                lblTotalAccountsVal.Text = result != DBNull.Value ? result.ToString() : "0";
                            }

                            // 3. Total Staff
                            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Role = 'Staff'", conn))
                            {
                                object result = cmd.ExecuteScalar();
                                lblTotalEmployeesVal.Text = result != DBNull.Value ? result.ToString() : "0";
                            }
                        }
                        catch { /* Tables might not exist yet */ }

                        // 4. Total Transactions and DataGridView
                        try
                        {
                            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Transactions", conn))
                            {
                                object result = cmd.ExecuteScalar();
                                lblTotalTransactionsVal.Text = result != DBNull.Value ? result.ToString() : "0";
                            }

                            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT TOP 5 * FROM Transactions ORDER BY TransactionDate DESC", conn))
                            {
                                DataTable dt = new DataTable();
                                adapter.Fill(dt);
                                dgvRecentTransactions.DataSource = dt;
                            }
                        }
                        catch { /* Tables might not exist yet */ }
                    }
                }
                catch (Exception ex)
                {
                    // Fail silently or show minimal error if DB connection completely fails
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void DashboardForm_Load_1(object sender, EventArgs e)
        {

        }

        private void panelCard4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
