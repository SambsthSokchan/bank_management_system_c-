using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace bank_management_system
{
    public partial class TransactionForm : Form
    {
        private string currentUserRole;
        private string currentUsername;

        public TransactionForm(string role = "Admin", string username = "")
        {
            InitializeComponent();
            currentUserRole = role;
            currentUsername = username;
            this.Load += new EventHandler(TransactionForm_Load);
            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
            this.btnDeleteTransaction.Click += new EventHandler(btnDeleteTransaction_Click);
            this.btnNewTransaction.Click += new EventHandler(btnNewTransaction_Click);
            this.btnViewTransaction.Click += new EventHandler(btnViewTransaction_Click);
            this.btnSearch.Click += new EventHandler(btnSearch_Click);
        }

        private void TransactionForm_Load(object sender, EventArgs e)
        {
            if (currentUserRole == "Customer")
            {
                btnDeleteTransaction.Visible = false;
                btnNewTransaction.Text = "Transfer Money";
            }
            LoadTransactions("");
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            LoadTransactions("");
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadTransactions(txtSearch.Text.Trim());
        }

        private void LoadTransactions(string filter)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["BankDbConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Join with Accounts to show AccountNumber instead of just ID for better UX
                    string query = @"
                        SELECT t.TransactionID, a.AccountNumber, t.TransactionType, 
                        CASE WHEN t.TransactionType = 'Withdrawal' THEN -t.Amount ELSE t.Amount END AS Amount, 
                        t.Description, t.TransactionDate 
                        FROM Transactions t
                        JOIN Accounts a ON t.AccountID = a.AccountID
                        WHERE (@Filter = '' OR a.AccountNumber LIKE '%' + @Filter + '%' OR t.TransactionType LIKE '%' + @Filter + '%' OR ISNULL(t.Description, '') LIKE '%' + @Filter + '%')";
                        
                    if (currentUserRole == "Customer")
                    {
                        query += " AND a.CustomerID IN (SELECT CustomerID FROM Customers WHERE UserID IN (SELECT UserID FROM Users WHERE Username = @Username))";
                    }

                    query += " ORDER BY t.TransactionDate DESC";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@Filter", filter);
                        if (currentUserRole == "Customer")
                        {
                            adapter.SelectCommand.Parameters.AddWithValue("@Username", currentUsername);
                        }
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvTransactions.DataSource = dt;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading transactions: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnNewTransaction_Click(object sender, EventArgs e)
        {
            using (TransactionEditDialog dialog = new TransactionEditDialog(currentUserRole, currentUsername))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    LoadTransactions("");
                }
            }
        }

        private void btnViewTransaction_Click(object sender, EventArgs e)
        {
            if (dgvTransactions.SelectedRows.Count > 0)
            {
                var row = dgvTransactions.SelectedRows[0];
                string accountNumber = row.Cells["AccountNumber"].Value?.ToString();
                string type = row.Cells["TransactionType"].Value?.ToString();
                decimal amount = Convert.ToDecimal(row.Cells["Amount"].Value);
                string description = row.Cells["Description"].Value?.ToString();

                using (TransactionEditDialog dialog = new TransactionEditDialog(currentUserRole, currentUsername))
                {
                    dialog.LoadTransactionData(accountNumber, type, amount, description);
                    dialog.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Please select a transaction to view.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDeleteTransaction_Click(object sender, EventArgs e)
        {
            if (dgvTransactions.SelectedRows.Count > 0)
            {
                var transactionId = dgvTransactions.SelectedRows[0].Cells["TransactionID"].Value;
                if (transactionId != null)
                {
                    DialogResult result = MessageBox.Show($"Are you sure you want to permanently delete Transaction ID {transactionId}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            string connectionString = ConfigurationManager.ConnectionStrings["BankDbConnection"].ConnectionString;
                            using (SqlConnection conn = new SqlConnection(connectionString))
                            {
                                conn.Open();
                                using (SqlCommand cmd = new SqlCommand("DELETE FROM Transactions WHERE TransactionID = @TransactionID", conn))
                                {
                                    cmd.Parameters.AddWithValue("@TransactionID", transactionId);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            LoadTransactions("");
                            MessageBox.Show("Transaction successfully deleted.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error deleting transaction: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a transaction to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
