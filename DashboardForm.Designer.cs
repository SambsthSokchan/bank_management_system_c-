namespace bank_management_system
{
    partial class DashboardForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblDashboardTitle = new System.Windows.Forms.Label();
            this.panelCard1 = new System.Windows.Forms.Panel();
            this.lblTotalCustomersVal = new System.Windows.Forms.Label();
            this.lblTotalCustomers = new System.Windows.Forms.Label();
            this.panelCard2 = new System.Windows.Forms.Panel();
            this.lblTotalAccountsVal = new System.Windows.Forms.Label();
            this.lblTotalAccounts = new System.Windows.Forms.Label();
            this.panelCard3 = new System.Windows.Forms.Panel();
            this.lblTotalEmployeesVal = new System.Windows.Forms.Label();
            this.lblTotalEmployees = new System.Windows.Forms.Label();
            this.panelCard4 = new System.Windows.Forms.Panel();
            this.lblTotalTransactionsVal = new System.Windows.Forms.Label();
            this.lblTotalTransactions = new System.Windows.Forms.Label();
            this.lblRecentTransactions = new System.Windows.Forms.Label();
            this.dgvRecentTransactions = new System.Windows.Forms.DataGridView();
            this.panelCard1.SuspendLayout();
            this.panelCard2.SuspendLayout();
            this.panelCard3.SuspendLayout();
            this.panelCard4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecentTransactions)).BeginInit();
            this.SuspendLayout();
            // 
            // lblDashboardTitle
            // 
            this.lblDashboardTitle.AutoSize = true;
            this.lblDashboardTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDashboardTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblDashboardTitle.Location = new System.Drawing.Point(20, 20);
            this.lblDashboardTitle.Name = "lblDashboardTitle";
            this.lblDashboardTitle.Size = new System.Drawing.Size(241, 32);
            this.lblDashboardTitle.TabIndex = 0;
            this.lblDashboardTitle.Text = "Dashboard Overview";
            // 
            // panelCard1
            // 
            this.panelCard1.BackColor = System.Drawing.Color.White;
            this.panelCard1.Controls.Add(this.lblTotalCustomersVal);
            this.panelCard1.Controls.Add(this.lblTotalCustomers);
            this.panelCard1.Location = new System.Drawing.Point(26, 75);
            this.panelCard1.Name = "panelCard1";
            this.panelCard1.Size = new System.Drawing.Size(185, 110);
            this.panelCard1.TabIndex = 1;
            // 
            // lblTotalCustomersVal
            // 
            this.lblTotalCustomersVal.AutoSize = true;
            this.lblTotalCustomersVal.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalCustomersVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.lblTotalCustomersVal.Location = new System.Drawing.Point(15, 45);
            this.lblTotalCustomersVal.Name = "lblTotalCustomersVal";
            this.lblTotalCustomersVal.Size = new System.Drawing.Size(38, 45);
            this.lblTotalCustomersVal.TabIndex = 1;
            this.lblTotalCustomersVal.Text = "0";
            // 
            // lblTotalCustomers
            // 
            this.lblTotalCustomers.AutoSize = true;
            this.lblTotalCustomers.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalCustomers.ForeColor = System.Drawing.Color.Gray;
            this.lblTotalCustomers.Location = new System.Drawing.Point(15, 15);
            this.lblTotalCustomers.Name = "lblTotalCustomers";
            this.lblTotalCustomers.Size = new System.Drawing.Size(117, 20);
            this.lblTotalCustomers.TabIndex = 0;
            this.lblTotalCustomers.Text = "Total Customers";
            // 
            // panelCard2
            // 
            this.panelCard2.BackColor = System.Drawing.Color.White;
            this.panelCard2.Controls.Add(this.lblTotalAccountsVal);
            this.panelCard2.Controls.Add(this.lblTotalAccounts);
            this.panelCard2.Location = new System.Drawing.Point(236, 75);
            this.panelCard2.Name = "panelCard2";
            this.panelCard2.Size = new System.Drawing.Size(185, 110);
            this.panelCard2.TabIndex = 2;
            // 
            // lblTotalAccountsVal
            // 
            this.lblTotalAccountsVal.AutoSize = true;
            this.lblTotalAccountsVal.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalAccountsVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.lblTotalAccountsVal.Location = new System.Drawing.Point(15, 45);
            this.lblTotalAccountsVal.Name = "lblTotalAccountsVal";
            this.lblTotalAccountsVal.Size = new System.Drawing.Size(38, 45);
            this.lblTotalAccountsVal.TabIndex = 1;
            this.lblTotalAccountsVal.Text = "0";
            // 
            // lblTotalAccounts
            // 
            this.lblTotalAccounts.AutoSize = true;
            this.lblTotalAccounts.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalAccounts.ForeColor = System.Drawing.Color.Gray;
            this.lblTotalAccounts.Location = new System.Drawing.Point(15, 15);
            this.lblTotalAccounts.Name = "lblTotalAccounts";
            this.lblTotalAccounts.Size = new System.Drawing.Size(108, 20);
            this.lblTotalAccounts.TabIndex = 0;
            this.lblTotalAccounts.Text = "Total Accounts";
            // 
            // panelCard3
            // 
            this.panelCard3.BackColor = System.Drawing.Color.White;
            this.panelCard3.Controls.Add(this.lblTotalEmployeesVal);
            this.panelCard3.Controls.Add(this.lblTotalEmployees);
            this.panelCard3.Location = new System.Drawing.Point(446, 75);
            this.panelCard3.Name = "panelCard3";
            this.panelCard3.Size = new System.Drawing.Size(185, 110);
            this.panelCard3.TabIndex = 3;
            // 
            // lblTotalEmployeesVal
            // 
            this.lblTotalEmployeesVal.AutoSize = true;
            this.lblTotalEmployeesVal.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalEmployeesVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.lblTotalEmployeesVal.Location = new System.Drawing.Point(15, 45);
            this.lblTotalEmployeesVal.Name = "lblTotalEmployeesVal";
            this.lblTotalEmployeesVal.Size = new System.Drawing.Size(38, 45);
            this.lblTotalEmployeesVal.TabIndex = 1;
            this.lblTotalEmployeesVal.Text = "0";
            // 
            // lblTotalEmployees
            // 
            this.lblTotalEmployees.AutoSize = true;
            this.lblTotalEmployees.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalEmployees.ForeColor = System.Drawing.Color.Gray;
            this.lblTotalEmployees.Location = new System.Drawing.Point(15, 15);
            this.lblTotalEmployees.Name = "lblTotalEmployees";
            this.lblTotalEmployees.Size = new System.Drawing.Size(119, 20);
            this.lblTotalEmployees.TabIndex = 0;
            this.lblTotalEmployees.Text = "Total Employees";
            // 
            // panelCard4
            // 
            this.panelCard4.BackColor = System.Drawing.Color.White;
            this.panelCard4.Controls.Add(this.lblTotalTransactionsVal);
            this.panelCard4.Controls.Add(this.lblTotalTransactions);
            this.panelCard4.Location = new System.Drawing.Point(656, 75);
            this.panelCard4.Name = "panelCard4";
            this.panelCard4.Size = new System.Drawing.Size(185, 110);
            this.panelCard4.TabIndex = 4;
            this.panelCard4.Paint += new System.Windows.Forms.PaintEventHandler(this.panelCard4_Paint);
            // 
            // lblTotalTransactionsVal
            // 
            this.lblTotalTransactionsVal.AutoSize = true;
            this.lblTotalTransactionsVal.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalTransactionsVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(68)))), ((int)(((byte)(173)))));
            this.lblTotalTransactionsVal.Location = new System.Drawing.Point(15, 45);
            this.lblTotalTransactionsVal.Name = "lblTotalTransactionsVal";
            this.lblTotalTransactionsVal.Size = new System.Drawing.Size(38, 45);
            this.lblTotalTransactionsVal.TabIndex = 1;
            this.lblTotalTransactionsVal.Text = "0";
            // 
            // lblTotalTransactions
            // 
            this.lblTotalTransactions.AutoSize = true;
            this.lblTotalTransactions.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalTransactions.ForeColor = System.Drawing.Color.Gray;
            this.lblTotalTransactions.Location = new System.Drawing.Point(15, 15);
            this.lblTotalTransactions.Name = "lblTotalTransactions";
            this.lblTotalTransactions.Size = new System.Drawing.Size(130, 20);
            this.lblTotalTransactions.TabIndex = 0;
            this.lblTotalTransactions.Text = "Total Transactions";
            // 
            // lblRecentTransactions
            // 
            this.lblRecentTransactions.AutoSize = true;
            this.lblRecentTransactions.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecentTransactions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblRecentTransactions.Location = new System.Drawing.Point(21, 215);
            this.lblRecentTransactions.Name = "lblRecentTransactions";
            this.lblRecentTransactions.Size = new System.Drawing.Size(180, 25);
            this.lblRecentTransactions.TabIndex = 5;
            this.lblRecentTransactions.Text = "Recent Transactions";
            // 
            // dgvRecentTransactions
            // 
            this.dgvRecentTransactions.AllowUserToAddRows = false;
            this.dgvRecentTransactions.AllowUserToDeleteRows = false;
            this.dgvRecentTransactions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRecentTransactions.BackgroundColor = System.Drawing.Color.White;
            this.dgvRecentTransactions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvRecentTransactions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRecentTransactions.Location = new System.Drawing.Point(26, 255);
            this.dgvRecentTransactions.Name = "dgvRecentTransactions";
            this.dgvRecentTransactions.ReadOnly = true;
            this.dgvRecentTransactions.Size = new System.Drawing.Size(815, 195);
            this.dgvRecentTransactions.TabIndex = 6;
            // 
            // DashboardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(864, 476);
            this.Controls.Add(this.dgvRecentTransactions);
            this.Controls.Add(this.lblRecentTransactions);
            this.Controls.Add(this.panelCard4);
            this.Controls.Add(this.panelCard3);
            this.Controls.Add(this.panelCard2);
            this.Controls.Add(this.panelCard1);
            this.Controls.Add(this.lblDashboardTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DashboardForm";
            this.Text = "DashboardForm";
            this.Load += new System.EventHandler(this.DashboardForm_Load_1);
            this.panelCard1.ResumeLayout(false);
            this.panelCard1.PerformLayout();
            this.panelCard2.ResumeLayout(false);
            this.panelCard2.PerformLayout();
            this.panelCard3.ResumeLayout(false);
            this.panelCard3.PerformLayout();
            this.panelCard4.ResumeLayout(false);
            this.panelCard4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecentTransactions)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblDashboardTitle;
        private System.Windows.Forms.Panel panelCard1;
        private System.Windows.Forms.Label lblTotalCustomersVal;
        private System.Windows.Forms.Label lblTotalCustomers;
        private System.Windows.Forms.Panel panelCard2;
        private System.Windows.Forms.Label lblTotalAccountsVal;
        private System.Windows.Forms.Label lblTotalAccounts;
        private System.Windows.Forms.Panel panelCard3;
        private System.Windows.Forms.Label lblTotalEmployeesVal;
        private System.Windows.Forms.Label lblTotalEmployees;
        private System.Windows.Forms.Panel panelCard4;
        private System.Windows.Forms.Label lblTotalTransactionsVal;
        private System.Windows.Forms.Label lblTotalTransactions;
        private System.Windows.Forms.Label lblRecentTransactions;
        private System.Windows.Forms.DataGridView dgvRecentTransactions;
    }
}
