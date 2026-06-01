using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace bank_management_system
{
    public partial class TransactionEditDialog : Form
    {
        private string currentUserRole;
        private string currentUsername;

        public TransactionEditDialog(string role = "Admin", string username = "")
        {
            InitializeComponent();
            currentUserRole = role;
            currentUsername = username;
            this.Load += new EventHandler(TransactionEditDialog_Load);
        }

        private void TransactionEditDialog_Load(object sender, EventArgs e)
        {
            LoadAccounts();
            
            if (currentUserRole == "Customer")
            {
                // User requirement: Support Deposit and Transfer for Customer role
                cmbType.Items.Clear();
                cmbType.Items.AddRange(new object[] { "Deposit", "Transfer" });
                cmbType.SelectedIndex = 0;
            }
            else
            {
                cmbType.SelectedIndex = 0; // Default to Deposit for Admin (who also sees Withdrawal)
            }
        }

        private void LoadAccounts()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["BankDbConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT AccountID, AccountNumber FROM Accounts WHERE Status = 'Active'";
                    if (currentUserRole == "Customer")
                    {
                        query += " AND CustomerID IN (SELECT CustomerID FROM Customers WHERE UserID IN (SELECT UserID FROM Users WHERE Username = @Username))";
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        if (currentUserRole == "Customer")
                        {
                            adapter.SelectCommand.Parameters.AddWithValue("@Username", currentUsername);
                        }
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        cmbAccount.DataSource = dt;
                        cmbAccount.DisplayMember = "AccountNumber";
                        cmbAccount.ValueMember = "AccountID";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading accounts: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbType.SelectedItem != null && cmbType.SelectedItem.ToString() == "Transfer")
            {
                lblDestinationAccount.Visible = true;
                txtDestinationAccount.Visible = true;
                lblRecipientName.Visible = true;
            }
            else
            {
                lblDestinationAccount.Visible = false;
                txtDestinationAccount.Visible = false;
                lblRecipientName.Visible = false;
                txtDestinationAccount.Text = "";
                lblRecipientName.Text = "";
            }
        }

        public void LoadTransactionData(string accountNumber, string type, decimal amount, string description)
        {
            // For view mode, we don't necessarily need to hit the database for the dropdown,
            // we can just temporarily add the account number to the dropdown and select it.
            cmbAccount.DataSource = null;
            cmbAccount.Items.Clear();
            cmbAccount.Items.Add(accountNumber);
            cmbAccount.SelectedIndex = 0;

            cmbType.SelectedItem = type;
            txtAmount.Text = amount.ToString("0.00");
            txtDescription.Text = description;

            this.Text = "View Transaction";
            lblTitle.Text = "Transaction Details";

            // Disable editing
            cmbAccount.Enabled = false;
            cmbType.Enabled = false;
            txtAmount.ReadOnly = true;
            txtDescription.ReadOnly = true;
            
            // Hide submit button, center close button
            btnSave.Visible = false;
            btnCancel.Text = "Close";
            btnCancel.Left = (this.ClientSize.Width - btnCancel.Width) / 2;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void txtDestinationAccount_TextChanged(object sender, EventArgs e)
        {
            string accNum = txtDestinationAccount.Text.Trim();
            if (string.IsNullOrWhiteSpace(accNum))
            {
                lblRecipientName.Text = "";
                return;
            }

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["BankDbConnection"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT c.FullName 
                                     FROM Accounts a 
                                     JOIN Customers c ON a.CustomerID = c.CustomerID 
                                     WHERE a.AccountNumber = @AccNum AND a.Status = 'Active'";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccNum", accNum);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            lblRecipientName.Text = "Recipient: " + result.ToString();
                            lblRecipientName.ForeColor = System.Drawing.Color.Green;
                        }
                        else
                        {
                            lblRecipientName.Text = "Account not found";
                            lblRecipientName.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                }
            }
            catch
            {
                lblRecipientName.Text = "";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cmbAccount.Text) || 
                cmbType.SelectedItem == null ||
                string.IsNullOrWhiteSpace(txtAmount.Text))
            {
                MessageBox.Show("Account, Type, and Amount are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbType.SelectedItem.ToString() == "Transfer" && string.IsNullOrWhiteSpace(txtDestinationAccount.Text))
            {
                MessageBox.Show("Destination Account is required for transfers.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Amount must be a valid positive number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string type = cmbType.SelectedItem.ToString();
            string description = txtDescription.Text;
            
            string connectionString = ConfigurationManager.ConnectionStrings["BankDbConnection"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get source AccountID
                    int accountId = 0;
                    if (cmbAccount.SelectedValue != null && cmbAccount.SelectedValue is int)
                    {
                        accountId = (int)cmbAccount.SelectedValue;
                    }
                    else
                    {
                        using (SqlCommand accCmd = new SqlCommand("SELECT AccountID FROM Accounts WHERE AccountNumber = @AccNum AND Status = 'Active'", conn))
                        {
                            accCmd.Parameters.AddWithValue("@AccNum", cmbAccount.Text.Trim());
                            object accResult = accCmd.ExecuteScalar();
                            if (accResult == null)
                            {
                                MessageBox.Show("Source account not found or is inactive.", "Transaction Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            accountId = Convert.ToInt32(accResult);
                        }
                    }

                    // Check balance if withdrawal or transfer
                    if (type == "Withdrawal" || type == "Transfer")
                    {
                        decimal currentBalance = 0;
                        using (SqlCommand checkCmd = new SqlCommand("SELECT Balance FROM Accounts WHERE AccountID = @AccountID", conn))
                        {
                            checkCmd.Parameters.AddWithValue("@AccountID", accountId);
                            object result = checkCmd.ExecuteScalar();
                            if (result != null)
                            {
                                currentBalance = Convert.ToDecimal(result);
                            }
                        }

                        if (amount > currentBalance)
                        {
                            MessageBox.Show("Insufficient funds for this transaction.", "Transaction Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    int destinationAccountId = 0;
                    if (type == "Transfer")
                    {
                        using (SqlCommand destCmd = new SqlCommand("SELECT AccountID FROM Accounts WHERE AccountNumber = @AccNum AND Status = 'Active'", conn))
                        {
                            destCmd.Parameters.AddWithValue("@AccNum", txtDestinationAccount.Text.Trim());
                            object result = destCmd.ExecuteScalar();
                            if (result == null)
                            {
                                MessageBox.Show("Destination account not found or is inactive.", "Transaction Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            destinationAccountId = Convert.ToInt32(result);
                        }
                        
                        if (destinationAccountId == accountId)
                        {
                            MessageBox.Show("Cannot transfer to the same account.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Start a transaction to ensure both insert and balance update succeed
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            if (type == "Transfer")
                            {
                                // 1. Withdraw from source
                                string insertOutQuery = @"INSERT INTO Transactions (AccountID, TransactionType, Amount, Description, TransactionDate) 
                                                     VALUES (@AccountID, 'Withdrawal', @Amount, @Description, GETDATE())";
                                using (SqlCommand cmd = new SqlCommand(insertOutQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@AccountID", accountId);
                                    cmd.Parameters.AddWithValue("@Amount", amount);
                                    cmd.Parameters.AddWithValue("@Description", string.IsNullOrWhiteSpace(description) ? "Transfer Out" : description);
                                    cmd.ExecuteNonQuery();
                                }
                                
                                // 2. Deposit to destination
                                string insertInQuery = @"INSERT INTO Transactions (AccountID, TransactionType, Amount, Description, TransactionDate) 
                                                     VALUES (@DestID, 'Deposit', @Amount, @Description, GETDATE())";
                                using (SqlCommand cmd = new SqlCommand(insertInQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@DestID", destinationAccountId);
                                    cmd.Parameters.AddWithValue("@Amount", amount);
                                    cmd.Parameters.AddWithValue("@Description", "Transfer In from " + cmbAccount.Text);
                                    cmd.ExecuteNonQuery();
                                }
                                
                                // 3. Update Source Balance
                                using (SqlCommand cmd = new SqlCommand("UPDATE Accounts SET Balance = Balance - @Amount WHERE AccountID = @AccountID", conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@Amount", amount);
                                    cmd.Parameters.AddWithValue("@AccountID", accountId);
                                    cmd.ExecuteNonQuery();
                                }
                                
                                // 4. Update Destination Balance
                                using (SqlCommand cmd = new SqlCommand("UPDATE Accounts SET Balance = Balance + @Amount WHERE AccountID = @DestID", conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@Amount", amount);
                                    cmd.Parameters.AddWithValue("@DestID", destinationAccountId);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // Normal Deposit/Withdrawal
                                string insertQuery = @"INSERT INTO Transactions (AccountID, TransactionType, Amount, Description, TransactionDate) 
                                                     VALUES (@AccountID, @TransactionType, @Amount, @Description, GETDATE())";
                                
                                using (SqlCommand cmd = new SqlCommand(insertQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@AccountID", accountId);
                                    cmd.Parameters.AddWithValue("@TransactionType", type);
                                    cmd.Parameters.AddWithValue("@Amount", amount);
                                    cmd.Parameters.AddWithValue("@Description", string.IsNullOrWhiteSpace(description) ? (object)DBNull.Value : description);
                                    cmd.ExecuteNonQuery();
                                }

                                string updateQuery = "";
                                if (type == "Deposit")
                                {
                                    updateQuery = "UPDATE Accounts SET Balance = Balance + @Amount WHERE AccountID = @AccountID";
                                }
                                else if (type == "Withdrawal")
                                {
                                    updateQuery = "UPDATE Accounts SET Balance = Balance - @Amount WHERE AccountID = @AccountID";
                                }

                                using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@Amount", amount);
                                    cmd.Parameters.AddWithValue("@AccountID", accountId);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // Commit
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception("Transaction rolled back due to error: " + ex.Message);
                        }
                    }
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
