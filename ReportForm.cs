using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;

namespace bank_management_system
{
    public partial class ReportForm : Form
    {
        public ReportForm()
        {
            InitializeComponent();
            this.Load += new EventHandler(ReportForm_Load);
            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
            this.btnExportExcel.Click += new EventHandler(btnExportExcel_Click);
            this.btnExportPdf.Click += new EventHandler(btnExportPdf_Click);
            this.btnSearch.Click += new EventHandler(btnSearch_Click);
        }

        private void ReportForm_Load(object sender, EventArgs e)
        {
            LoadReportData("");
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            LoadReportData("");
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadReportData(txtSearch.Text.Trim());
        }

        private void LoadReportData(string filter)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["BankDbConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            c.FullName AS 'Customer Name', 
                            a.AccountNumber AS 'Account Number', 
                            a.AccountType AS 'Account Type', 
                            a.Balance AS 'Current Balance', 
                            COUNT(t.TransactionID) AS 'Total Transactions'
                        FROM Accounts a
                        JOIN Customers c ON a.CustomerID = c.CustomerID
                        LEFT JOIN Transactions t ON a.AccountID = t.AccountID
                        WHERE (@Filter = '' OR c.FullName LIKE '%' + @Filter + '%')
                        GROUP BY c.FullName, a.AccountNumber, a.AccountType, a.Balance
                        ORDER BY a.Balance DESC";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@Filter", filter);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvReports.DataSource = dt;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error generating report: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (dgvReports.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.", "Empty Report", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV File (*.csv)|*.csv";
            sfd.FileName = "FinancialReport_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();

                    // Headers
                    string[] columnNames = new string[dgvReports.Columns.Count];
                    for (int i = 0; i < dgvReports.Columns.Count; i++)
                    {
                        columnNames[i] = dgvReports.Columns[i].HeaderText;
                    }
                    sb.AppendLine(string.Join(",", columnNames));

                    // Rows
                    foreach (DataGridViewRow row in dgvReports.Rows)
                    {
                        string[] cells = new string[dgvReports.Columns.Count];
                        for (int i = 0; i < dgvReports.Columns.Count; i++)
                        {
                            cells[i] = row.Cells[i].Value?.ToString().Replace(",", " "); // Prevent CSV shifting
                        }
                        sb.AppendLine(string.Join(",", cells));
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString());
                    MessageBox.Show("Report successfully exported to Excel (CSV format)!", "Export Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error exporting data: " + ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            if (dgvReports.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.", "Empty Report", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(PrintReportPage);
            
            PrintDialog printDlg = new PrintDialog();
            printDlg.Document = pd;
            
            // This allows the user to select 'Microsoft Print to PDF'
            if (printDlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pd.Print();
                    MessageBox.Show("Report successfully sent to printer/PDF!", "Export Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error printing report: " + ex.Message, "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void PrintReportPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font fontTitle = new Font("Segoe UI", 18, FontStyle.Bold);
            Font fontHeader = new Font("Segoe UI", 12, FontStyle.Bold);
            Font fontBody = new Font("Segoe UI", 10, FontStyle.Regular);
            Brush brush = Brushes.Black;

            int yPos = 50;
            int xPos = 50;

            // Draw Title
            g.DrawString("Financial Report - Bank Management System", fontTitle, brush, xPos, yPos);
            yPos += 50;

            // Draw Headers
            int[] columnWidths = { 200, 150, 120, 120, 150 };
            for (int i = 0; i < dgvReports.Columns.Count; i++)
            {
                g.DrawString(dgvReports.Columns[i].HeaderText, fontHeader, brush, xPos, yPos);
                xPos += columnWidths[i];
            }
            
            yPos += 30;
            g.DrawLine(Pens.Black, 50, yPos, e.PageBounds.Width - 50, yPos);
            yPos += 10;

            // Draw Rows
            foreach (DataGridViewRow row in dgvReports.Rows)
            {
                xPos = 50;
                for (int i = 0; i < dgvReports.Columns.Count; i++)
                {
                    string cellValue = row.Cells[i].Value?.ToString() ?? "";
                    g.DrawString(cellValue, fontBody, brush, xPos, yPos);
                    xPos += columnWidths[i];
                }
                yPos += 25;

                // Move to next page if running out of space
                if (yPos > e.PageBounds.Height - 50)
                {
                    e.HasMorePages = true;
                    return;
                }
            }
            e.HasMorePages = false;
        }
    }
}
