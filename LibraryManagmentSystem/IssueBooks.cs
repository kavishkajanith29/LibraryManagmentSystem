using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using ZXing.Presentation;
using System.Text.RegularExpressions;

namespace LibraryManagmentSystem
{
    public partial class IssueBooks : Form
    {
        SqlConnection conn = new SqlConnection("Data Source=KAVISHKAJANITH\\SQLEXPRESS;Initial Catalog=LibraryManagmentSystem;Integrated Security=True");
        private string qr = string.Empty;
        private FilterInfoCollection filterInfoCollection;
        public static VideoCaptureDevice captureDevice;
        public IssueBooks()
        {
            InitializeComponent();

            txtLid.TextChanged+= txtLid_TextChanged;
            txtISBN.TextChanged+= txtISBN_TextChanged;
            
        }

        private void IssueBooks_Load(object sender, EventArgs e)
        {
            panel2.Visible = false;
            /*filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filterInfoCollection)
                cmbDevice.Items.Add(filterInfo.Name);
            cmbDevice.SelectedIndex = 0;*/
        }

        private void UserSearch()
        {
            string quary1 = "SELECT L_name FROM LibraryUsers WHERE L_id = '"+txtLid.Text+"'";
            using (SqlCommand cmd1 = new SqlCommand(quary1, conn))
            {
                conn.Open();
                object result = cmd1.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    conn.Close();
                    lblUserName.Text = result.ToString();
                    BookLimit();
                }
                else
                {
                    conn.Close();
                    lblUserName.Text = "User Was Not Found!";
                }
            }
        }
        private void btnIssue_Click(object sender, EventArgs e)
        {
            if (txtISBN.Text !="" && txtLid.Text!="" && lblLimit.Text != "The member has already issued three books.\nCannot issue more.")
            {
                AvailibleBooks();
            }
            else
            {
                MessageBox.Show("Please Fill the Informatin to Issue the Book.","Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        private void AvailibleBooks()
        {
            string quary1 = "SELECT b_quantity FROM NewBook WHERE b_isbn =@isbn";
            using (SqlCommand cmd1 = new SqlCommand(quary1, conn))
            {
                try
                {
                    conn.Open();
                    cmd1.Parameters.AddWithValue("@isbn", txtISBN.Text);
                    object result = cmd1.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                       int qty = Convert.ToInt32(result);
                        conn.Close();
                        if (qty > 0)
                        {
                            issueBooks();
                        }
                        else if(qty==0)
                        {
                            MessageBox.Show("Wrong ISBN ,\nThis Book was out of stock.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        }
                    }
                    else
                    {
                        conn.Close();
                        MessageBox.Show("Wrong ISBN ,\nBook was not found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch
                {

                }
            }
        }

        private void issueBooks()
        {
            try
            {
                if (lblLimit.Text != "The member has already issued three books.\nCannot issue more." && txtLid.Text != "" && txtISBN.Text != "")
                {
                    conn.Open();
                    string query = "INSERT INTO IssueBooks (b_isbn, L_id, isuue_date,return_date,late_fee) VALUES (@BookID, @MemberID, @IssueDate,NULL,NULL)";
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@BookID", txtISBN.Text);
                        command.Parameters.AddWithValue("@MemberID", txtLid.Text);
                        command.Parameters.AddWithValue("@IssueDate", DateTime.Now);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            conn.Close();
                            BookUpdate();
                        }
                        else
                        {
                            conn.Close();
                            MessageBox.Show("Failed to issue the book. Please check the inputs and try again.");
                        }

                    }
                }
                else
                {
                    MessageBox.Show("Please Fill the Informatin to Issue the Book.");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex+"");
            }
        }

        private void BookUpdate()
        {
            string QuantityQuery = "UPDATE NewBook SET b_quantity -= 1  WHERE b_isbn = @isbn";

            using (SqlCommand cmdQntity = new SqlCommand(QuantityQuery, conn))
            {
                conn.Open();
                cmdQntity.Parameters.AddWithValue("@isbn", txtISBN.Text);
                int rowsAffected1 = cmdQntity.ExecuteNonQuery();
                if (rowsAffected1 > 0)
                {
                    MessageBox.Show("Book issued successfully.");
                    lblUserName.Text = "";
                    lblCategory.Text = "";
                    lblName.Text = "";
                    lblAuthor.Text = "";
                    txtISBN.Text = string.Empty;
                    txtLid.Text = string.Empty;
                    conn.Close();
                }
                else
                {
                    conn.Close();
                    MessageBox.Show("Failed to issue the book.");
                }
            }
        }



        private void txtISBN_TextChanged(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                string query = "SELECT b_category, b_name, b_author FROM NewBook WHERE b_isbn = '" + txtISBN.Text + "'";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        lblCategory.Text = reader["b_category"].ToString();
                        lblName.Text = reader["b_name"].ToString();
                        lblAuthor.Text = reader["b_author"].ToString();
                    }
                    else
                    {
                        lblCategory.Text = "No Result Was Found!";
                        lblName.Text = "";
                        lblAuthor.Text = "";
                    }
                    reader.Close();
                }
            }
            catch
            {
                
            }
            finally
            {
                conn.Close();
            }
        }
        private void txtLid_TextChanged(object sender, EventArgs e)
        {
            UserSearch();
        }
        private void BookLimit() 
        {
            string countQuery = "SELECT COUNT(*) FROM IssueBooks WHERE L_id = @UserID AND return_date IS NULL";
            using (SqlCommand countcmd = new SqlCommand(countQuery, conn))
            {
                conn.Open();
                countcmd.Parameters.AddWithValue("@UserID", txtLid.Text);
                int issuedBooksCount = (int)countcmd.ExecuteScalar();
                if (issuedBooksCount >= 3)
                {
                    lblLimit.Text = "The User has already issued three books.\nCannot issue more.";
                }
                else
                {
                    lblLimit.Text = "";
                }
                conn.Close();
            }
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
                txtISBN.Text = qr;
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
        private void IssueBooks_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (captureDevice!=null && captureDevice.IsRunning)
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
