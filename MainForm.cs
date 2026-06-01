using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace bank_management_system
{
    public partial class MainForm : Form
    {
        private string currentUserRole;
        private string currentUsername;

        public MainForm(string role = "Admin mode", string username = "Admin")
        {
            InitializeComponent();
            currentUserRole = role;
            currentUsername = username;
            this.Load += new System.EventHandler(this.Form1_Load);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblCurrentRole.Text = currentUserRole;
            lblDateTime.Text = DateTime.Now.ToString("dd MMM yyyy, HH:mm");
            timerDateTime.Interval = 1000;
            timerDateTime.Start();

            // Set rounded corners for panels
            SetRoundedRegion(pnlRoleBadge, 20);
            SetRoundedRegion(pnlDateBadge, 20);
            SetRoundedRegion(pnlStatusDot, pnlStatusDot.Width);

            ApplyRolePermissions();

            // Load Dashboard by default
            LoadForm(new DashboardForm(currentUserRole, currentUsername));
        }

        private void ApplyRolePermissions()
        {
            if (currentUserRole.Equals("Staff", StringComparison.OrdinalIgnoreCase))
            {
                button5.Visible = false; // Manage User (hidden)
                
                // Shift the lower buttons up to fill the gap
                button6.Location = new Point(72, 414); // Move Reports to slot 5
                btnSignOut.Location = new Point(72, 478); // Move Sign Out to slot 6
            }
            else if (currentUserRole.Equals("Customer", StringComparison.OrdinalIgnoreCase))
            {
                // Customer wants: Dashboard, Accounts, Transactions, Profile
                button2.Visible = false; // Customers (hidden)
                
                // Accounts (button3)
                button3.Location = new Point(72, 205); // Move up to slot 2
                
                // Transactions (button4)
                button4.Location = new Point(72, 274); // Move up to slot 3
                
                button5.Visible = false; // Manage User (hidden)
                
                // Reports (button6) repurposed as Profile
                button6.Text = "Profile";
                button6.Location = new Point(72, 344); // Move up to slot 4
                
                // Sign Out (btnSignOut)
                btnSignOut.Location = new Point(72, 414); // Move up to slot 5
            }
        }

        private void timerDateTime_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("dd MMM yyyy, HH:mm");
        }

        private void SetRoundedRegion(Control control, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(control.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(control.Width - radius, control.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, control.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            control.Region = new Region(path);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void lblDateTime_Click(object sender, EventArgs e)
        {

        }

        private void LoadForm(Form form)
        {
            // Clear current content
            panelContent.Controls.Clear();

            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            panelContent.Controls.Add(form);
            panelContent.Tag = form;
            form.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Dashboard
            LoadForm(new DashboardForm(currentUserRole, currentUsername));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Customers
            LoadForm(new CustomerForm());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Accounts
            LoadForm(new AccountForm(currentUserRole, currentUsername));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Transactions
            LoadForm(new TransactionForm(currentUserRole, currentUsername));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Manage User
            LoadForm(new UserManagementForm());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (currentUserRole.Equals("Customer", StringComparison.OrdinalIgnoreCase))
            {
                // Profile button clicked for customer
                // Retrieve UserID or CustomerID from database to show Edit Dialog
                try
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["BankDbConnection"].ConnectionString;
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        // Get Customer ID for the current user
                        string query = @"SELECT c.CustomerID, u.Username, u.Email, c.FullName, c.Phone 
                                         FROM Customers c JOIN Users u ON c.UserID = u.UserID 
                                         WHERE u.Username = @User";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@User", currentUsername);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    int customerId = reader.GetInt32(0);
                                    string username = reader["Username"].ToString();
                                    string email = reader["Email"].ToString();
                                    string fullName = reader["FullName"].ToString();
                                    string phone = reader["Phone"].ToString();
                                    
                                    using (CustomerEditDialog dialog = new CustomerEditDialog())
                                    {
                                        dialog.IsEditMode = true;
                                        dialog.CustomerId = customerId;
                                        dialog.LoadCustomerData(username, email, fullName, phone);
                                        dialog.ShowDialog();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Profile not found! Please contact admin.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not load profile: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Reports
                LoadForm(new ReportForm());
            }
        }

        private void btnSignOut_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to sign out?", "Confirm Sign Out", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Restart();
                Environment.Exit(0);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
