using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace bank_management_system
{
    public partial class AccountForm : Form
    {
        private string currentUserRole;
        private string currentUsername;

        public AccountForm(string role = "Admin", string username = "")
        {
            InitializeComponent();
            currentUserRole = role;
            currentUsername = username;
            this.Load += new EventHandler(AccountForm_Load);
            this.btnRefresh.Click += new EventHandler(this.btnRefresh_Click);
            this.btnDeleteAccount.Click += new EventHandler(this.btnDeleteAccount_Click);
            this.btnAddAccount.Click += new EventHandler(this.btnAddAccount_Click);
            this.btnEditAccount.Click += new EventHandler(this.btnEditAccount_Click);
            this.btnSearch.Click += new EventHandler(this.btnSearch_Click);
        }

        private void AccountForm_Load(object sender, EventArgs e)
        {
            if (currentUserRole == "Customer")
            {
                lblTitle.Text = "My Accounts";
                btnAddAccount.Visible = false;
                btnEditAccount.Visible = false;
                btnDeleteAccount.Visible = false;
                
                // Hide search functionality for customer
                lblSearch.Visible = false;
                txtSearch.Visible = false;
                btnSearch.Visible = false;
            }
            LoadAccounts("");
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            LoadAccounts("");
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadAccounts(txtSearch.Text.Trim());
        }

        private void LoadAccounts(string filter)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["BankDbConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT a.AccountID, a.CustomerID, c.FullName AS CustomerName, a.AccountNumber, a.AccountType, a.Balance, a.Status, a.CreatedAt 
                                     FROM Accounts a
                                     JOIN Customers c ON a.CustomerID = c.CustomerID
                                     WHERE (@Filter = '' OR c.FullName LIKE '%' + @Filter + '%' OR a.AccountNumber LIKE '%' + @Filter + '%')";
                    
                    if (currentUserRole == "Customer")
                    {
                        query += " AND c.UserID IN (SELECT UserID FROM Users WHERE Username = @Username)";
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@Filter", filter);
                        if (currentUserRole == "Customer")
                        {
                            adapter.SelectCommand.Parameters.AddWithValue("@Username", currentUsername);
                        }
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvAccounts.DataSource = dt;

                        if (currentUserRole == "Customer")
                        {
                            if (dgvAccounts.Columns.Contains("AccountID")) dgvAccounts.Columns["AccountID"].Visible = false;
                            if (dgvAccounts.Columns.Contains("CustomerID")) dgvAccounts.Columns["CustomerID"].Visible = false;
                            if (dgvAccounts.Columns.Contains("CustomerName")) dgvAccounts.Columns["CustomerName"].Visible = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            using (AccountEditDialog dialog = new AccountEditDialog())
            {
                dialog.IsEditMode = false;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    LoadAccounts("");
                }
            }
        }

        private void btnEditAccount_Click(object sender, EventArgs e)
        {
            if (dgvAccounts.SelectedRows.Count > 0)
            {
                var row = dgvAccounts.SelectedRows[0];
                int accountId = Convert.ToInt32(row.Cells["AccountID"].Value);
                int customerId = Convert.ToInt32(row.Cells["CustomerID"].Value);
                string accountNumber = row.Cells["AccountNumber"].Value?.ToString();
                string accountType = row.Cells["AccountType"].Value?.ToString();
                decimal balance = Convert.ToDecimal(row.Cells["Balance"].Value);
                string status = row.Cells["Status"].Value?.ToString();

                using (AccountEditDialog dialog = new AccountEditDialog())
                {
                    dialog.IsEditMode = true;
                    dialog.AccountId = accountId;
                    dialog.LoadAccountData(customerId, accountNumber, accountType, balance, status);

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        LoadAccounts("");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an account to edit.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            if (dgvAccounts.SelectedRows.Count > 0)
            {
                var accountId = dgvAccounts.SelectedRows[0].Cells["AccountID"].Value;
                if (accountId != null)
                {
                    DialogResult result = MessageBox.Show($"Are you sure you want to CLOSE Account ID {accountId}?\n\n(Closing an account preserves its transaction history but prevents new transactions.)", "Confirm Close", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            string connectionString = ConfigurationManager.ConnectionStrings["BankDbConnection"].ConnectionString;
                            using (SqlConnection conn = new SqlConnection(connectionString))
                            {
                                conn.Open();
                                // We can delete or soft delete. Since Status exists, maybe update it.
                                using (SqlCommand cmd = new SqlCommand("UPDATE Accounts SET Status = 'Closed' WHERE AccountID = @AccountID", conn))
                                {
                                    cmd.Parameters.AddWithValue("@AccountID", accountId);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            LoadAccounts(""); // Refresh grid
                            MessageBox.Show("Account successfully closed.", "Account Closed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error closing account: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an account to close.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void AccountForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}
