using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LibraryManagmentSystem
{
    public partial class ReturnBooks : Form
    {
        private SqlConnection conn = new SqlConnection("Data Source=KAVISHKAJANITH\\SQLEXPRESS;Initial Catalog=LibraryManagmentSystem;Integrated Security=True");
        private string qr = "";
        private FilterInfoCollection filterInfoCollection;
        public static VideoCaptureDevice captureDevice;

        private static DateTime lateDate;
        private static double latefee;

        public ReturnBooks()
        {
            
            InitializeComponent();
            txtLid.TextChanged += txtLid_TextChanged;
        }
        
        private void ReturnBooks_Load(object sender, EventArgs e)
        {
            panel2.Visible = false;
        }
        private void UserSearch()
        {
            string quary1 = "SELECT l.L_name FROM LibraryUsers l,IssueBooks i WHERE l.L_id = i.L_id AND i.L_id =@Lid";
            using (SqlCommand cmd1 = new SqlCommand(quary1, conn))
            {
                try
                {
                    conn.Open();
                    cmd1.Parameters.AddWithValue("@Lid", txtLid.Text);
                    object result = cmd1.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        lblUserName.Text = result.ToString();
                    }
                    else
                    {
                        lblUserName.Text = "User Was Not Found!";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(" " + ex);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void txtLid_TextChanged(object sender, EventArgs e)
        {
            UserSearch();
            if (lblUserName.Text != "User Was Not Found!" && lblUserName.Text != string.Empty)
            {
                comboBox1.Enabled = true;
            }
        }



        private void btnReturn_Click(object sender, EventArgs e)
        {
            conn.Open();
            string query = "SELECT isuue_date FROM IssueBooks WHERE b_isbn = @BookID AND L_id = @MemberID AND return_date IS NULL";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@BookID", comboBox1.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@MemberID", txtLid.Text);

                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    DateTime issueDate = (DateTime)result;
                    DateTime returnDate = DateTime.Now;

                    TimeSpan overdueDuration = returnDate - issueDate;
                    int lateDays = overdueDuration.Days;
                    double lateFeePerDay = double.Parse(txtLtfee.Text);
                    double latefee = lateDays * lateFeePerDay;

                    if (lateDays > 0)
                    {
                        conn.Close();
                        lateReturn(latefee); 
                    }
                    else
                    {
                        conn.Close();
                        NolateReturn();
                    }
                }
                else
                {
                    conn.Close();
                    MessageBox.Show("No records found for this book and member.");
                }
            }
        }

        private void lateReturn(double fee)
        {
            conn.Open();
            string query1 = "UPDATE IssueBooks SET return_date = @rdate,late_fee = @lateFee WHERE L_id = @MemberID AND b_isbn = @isbn  AND return_date IS NULL";
            using (SqlCommand command = new SqlCommand(query1, conn))
            {
                command.Parameters.AddWithValue("@isbn", comboBox1.SelectedItem.ToString());
                command.Parameters.AddWithValue("@MemberID", txtLid.Text);
                command.Parameters.AddWithValue("@rdate", DateTime.Now);
                command.Parameters.AddWithValue("@lateFee", latefee);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    string query2 = "UPDATE NewBook SET b_quantity += 1 WHERE b_isbn = @bookID";
                    using (SqlCommand command2 = new SqlCommand(query1, conn))
                    {
                        command.Parameters.AddWithValue("@bookID", comboBox1.SelectedItem.ToString());
                        int rowsAffected2 = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            conn.Close();
                            MessageBox.Show("Book Return successfully.\nLate Fee = Rs."+latefee);
                        }
                        else
                        {
                            conn.Close();
                            MessageBox.Show("Fail to Return the Book.");
                        }
                    }
                }
                else
                {
                    conn.Close();
                    MessageBox.Show("Fail to Return the Book.");
                }
            }
        }
        private void NolateReturn()
        {
            conn.Open();
            string query1 = "UPDATE IssueBooks SET return_date = @rdate WHERE L_id = @MemberID AND b_isbn = @isbn  AND return_date IS NULL";
            using (SqlCommand command = new SqlCommand(query1, conn))
            {
                command.Parameters.AddWithValue("@isbn", comboBox1.SelectedItem.ToString());
                command.Parameters.AddWithValue("@MemberID", txtLid.Text);
                command.Parameters.AddWithValue("@rdate", DateTime.Now);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    string query2 = "UPDATE NewBook SET b_quantity += 1 WHERE b_isbn = @bookID";
                    using (SqlCommand command2 = new SqlCommand(query1, conn))
                    {
                        command.Parameters.AddWithValue("@bookID", comboBox1.SelectedItem.ToString());
                        int rowsAffected2 = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            conn.Close();
                            MessageBox.Show("Book Return successfully.");
                        }
                        else
                        {
                            conn.Close();
                            MessageBox.Show("Fail to Return the Book.");
                        }
                    }
                }
                else
                {
                    conn.Close();
                    MessageBox.Show("Fail to Return the Book.");
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            notReturned();
            
        }
        private void notReturned()
        {
            comboBox1.Items.Clear();
            conn.Open();
            string query = "SELECT b_isbn FROM IssueBooks WHERE L_id = @MemberID AND return_date IS NULL";
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@MemberID", txtLid.Text);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string item = reader["b_isbn"].ToString();
                        comboBox1.Items.Add(item);
                    }
                    conn.Close();
                }
            }
        }

        private void bookDetails()
        {
            conn.Open();
            string query = "SELECT b_category, b_name, b_author FROM NewBook WHERE b_isbn = @isbn";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@isbn", comboBox1.SelectedItem.ToString());
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblCategory.Text = reader["b_category"].ToString();
                    lblName.Text = reader["b_name"].ToString();
                    lblAuthor.Text = reader["b_author"].ToString();
                }
                else
                {
                    lblCategory.Text = "";
                    lblName.Text = "";
                    lblAuthor.Text = "";
                }
                reader.Close();
                conn.Close();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            bookDetails();
        }
        private void brnLidScan_Click(object sender, EventArgs e)
        {
            panel2.Visible = Visible;
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filterInfoCollection)
                cmbDevice.Items.Add(filterInfo.Name);
            cmbDevice.SelectedIndex = 0;
        }

        private void btnISBNScan_Click(object sender, EventArgs e)
        {
            panel2.Visible = Visible;
        }
        private void Qr(string scan)
        {
            string Pattern = @"^\d+$";

            bool Validisbn = Regex.IsMatch(qr, Pattern);
            if (Validisbn)
            {
                lblISBN.Text = qr;
            }
            else
            {
                txtLid.Text = qr;
            }
        }
        private void CaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
        }
        private void cmbDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            captureDevice = new VideoCaptureDevice(filterInfoCollection[cmbDevice.SelectedIndex].MonikerString);
            captureDevice.NewFrame += CaptureDevice_NewFrame;
            captureDevice.Start();
            timer1.Start();
        }
        private void btnScanOk_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            captureDevice.Stop();
            cmbDevice.Items.Clear();
            Qr(qr);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                ZXing.BarcodeReader barcodeReader = new ZXing.BarcodeReader();
                Result result = barcodeReader.Decode((Bitmap)pictureBox1.Image);
                if (result != null)
                {
                    qr = result.ToString();
                    timer1.Stop();
                    captureDevice.Stop();
                }
            }
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            if (captureDevice.IsRunning)
            {
                captureDevice.Stop();
                timer1.Stop();
            }
        }
        private void ReturnBooks_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (captureDevice.IsRunning)
            {
                captureDevice.Stop();
                timer1.Stop();
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are You Sure Sure?", "WARNING", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                this.Close();
            }
        }
    }
}
