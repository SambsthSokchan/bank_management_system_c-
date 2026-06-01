using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace bank_management_system
{
    public partial class AccountEditDialog : Form
    {
        public bool IsEditMode { get; set; } = false;
        public int AccountId { get; set; } = 0;

        public AccountEditDialog()
        {
            InitializeComponent();
            this.Load += new EventHandler(AccountEditDialog_Load);
        }

        private void AccountEditDialog_Load(object sender, EventArgs e)
        {
            LoadCustomers();

            if (!IsEditMode)
            {
                cmbAccountType.SelectedIndex = 0;
                cmbStatus.SelectedIndex = 0;
                
                // Auto-generate a random account number (e.g. ACC-1234567890)
                Random random = new Random();
                string prefix = "ACC-";
                string number = random.Next(10000, 99999).ToString() + random.Next(10000, 99999).ToString();
                txtAccountNumber.Text = prefix + number;
            }
        }

        private void LoadCustomers()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["BankDbConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT CustomerID, FullName FROM Customers WHERE Status = 'Active'", conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        cmbCustomer.DataSource = dt;
                        cmbCustomer.DisplayMember = "FullName";
                        cmbCustomer.ValueMember = "CustomerID";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading customers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void LoadAccountData(int customerId, string accountNumber, string accountType, decimal balance, string status)
        {
            // Ensure customers are loaded first before setting the value
            LoadCustomers();

            cmbCustomer.SelectedValue = customerId;
            txtAccountNumber.Text = accountNumber;
            cmbAccountType.SelectedItem = accountType;
            txtBalance.Text = balance.ToString("0.00");
            cmbStatus.SelectedItem = status;
            
            this.Text = "Edit Account";
            lblTitle.Text = "Edit Account";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbCustomer.SelectedValue == null || 
                string.IsNullOrWhiteSpace(txtAccountNumber.Text) || 
                cmbAccountType.SelectedItem == null ||
                string.IsNullOrWhiteSpace(txtBalance.Text) ||
                cmbStatus.SelectedItem == null)
            {
                MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtBalance.Text, out decimal balance))
            {
                MessageBox.Show("Balance must be a valid number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        string query = @"UPDATE Accounts SET CustomerID = @CustomerID, AccountNumber = @AccountNumber, 
                                         AccountType = @AccountType, Balance = @Balance, Status = @Status 
                                         WHERE AccountID = @AccountID";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@CustomerID", cmbCustomer.SelectedValue);
                            cmd.Parameters.AddWithValue("@AccountNumber", txtAccountNumber.Text);
                            cmd.Parameters.AddWithValue("@AccountType", cmbAccountType.SelectedItem.ToString());
                            cmd.Parameters.AddWithValue("@Balance", balance);
                            cmd.Parameters.AddWithValue("@Status", cmbStatus.SelectedItem.ToString());
                            cmd.Parameters.AddWithValue("@AccountID", AccountId);
                            
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string query = @"INSERT INTO Accounts (CustomerID, AccountNumber, AccountType, Balance, Status, CreatedAt) 
                                         VALUES (@CustomerID, @AccountNumber, @AccountType, @Balance, @Status, GETDATE())";
                        
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@CustomerID", cmbCustomer.SelectedValue);
                            cmd.Parameters.AddWithValue("@AccountNumber", txtAccountNumber.Text);
                            cmd.Parameters.AddWithValue("@AccountType", cmbAccountType.SelectedItem.ToString());
                            cmd.Parameters.AddWithValue("@Balance", balance);
                            cmd.Parameters.AddWithValue("@Status", cmbStatus.SelectedItem.ToString());
                            
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving account: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
