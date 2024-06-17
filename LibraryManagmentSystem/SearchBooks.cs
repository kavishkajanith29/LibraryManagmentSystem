using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryManagmentSystem
{
    public partial class SearchBooks : Form
    {
        SqlConnection conn = new SqlConnection("Data Source=KAVISHKAJANITH\\SQLEXPRESS;Initial Catalog=LibraryManagmentSystem;Integrated Security=True");
        private string searchquery;
        public SearchBooks()
        {
            InitializeComponent();
        }
        private void gridLoad()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM NewBook ", conn);
            SqlDataAdapter da1 = new SqlDataAdapter(cmd);
            DataSet ds1 = new DataSet();
            da1.Fill(ds1);

            dataGridView1.DataSource = ds1.Tables[0];
            comboBox1.SelectedIndex = 0;
        }
        private void Search_Load(object sender, EventArgs e)
        {
            panel1.Visible = false;
            gridLoad();
            
            pictureBox2.Visible = false;

            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.YellowGreen;
            dataGridView1.RowsDefaultCellStyle.BackColor = Color.YellowGreen;

            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige;

            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Green;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Yellow;
            dataGridView1.Columns[0].HeaderText = "ISBN";
            dataGridView1.Columns[1].HeaderText = "Book Category";
            dataGridView1.Columns[2].HeaderText = "Book Name";
            dataGridView1.Columns[3].HeaderText = "Book Author";
            dataGridView1.Columns[4].HeaderText = "Book Quantity";
            dataGridView1.Columns[5].HeaderText = "Book Updated Date";
            dataGridView1.Columns[0].Width = 100;
            dataGridView1.Columns[1].Width = 100;
            dataGridView1.Columns[2].Width = 200;
            dataGridView1.Columns[3].Width = 200;
            dataGridView1.Columns[4].Width = 107;
            dataGridView1.Columns[5].Width = 100;

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            
            
        }
        string clicked_ISBN;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 )
            {
                clicked_ISBN = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                panel1.Visible = true;
                string query = "SELECT * FROM NewBook WHERE b_ISBN =@ISBN";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@ISBN", clicked_ISBN);

                    SqlDataAdapter da1 = new SqlDataAdapter(cmd);
                    DataSet ds1 = new DataSet();
                    da1.Fill(ds1);

                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        conn.Close();
                        lblISBN.Text = ds1.Tables[0].Rows[0][0].ToString();
                        txtCatogory.Text = ds1.Tables[0].Rows[0][1].ToString();
                        txtName.Text = ds1.Tables[0].Rows[0][2].ToString();
                        txtAuthor.Text = ds1.Tables[0].Rows[0][3].ToString();
                        txtQuantity.Text = ds1.Tables[0].Rows[0][4].ToString();
                        
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Data Will Be Deleted,Confirm?", "Succsusfull", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    string query = "DELETE FROM NewBook Where b_ISBN =@ISBN ";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@ISBN", clicked_ISBN);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Book details  successfully Deleted.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            panel1.Visible = false;
                            conn.Close();
                            SqlCommand cmd2 = new SqlCommand("SELECT * FROM NewBook ", conn);
                            SqlDataAdapter da1 = new SqlDataAdapter(cmd2);
                            DataSet ds1 = new DataSet();
                            da1.Fill(ds1);

                            dataGridView1.DataSource = ds1.Tables[0];
                        }
                        else
                        {
                            MessageBox.Show("Failed to Update book details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception e1)
            {
                MessageBox.Show("" + e1, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Data Will Be Updated,Confirm?","Succsusfull",MessageBoxButtons.OKCancel,MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    string query = "UPDATE NewBook SET b_category = @category,b_name = @name,b_author = @author,b_quantity=@quantity,b_updated_date=@Adate WHERE b_isbn = @ISBN";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@ISBN",lblISBN.Text);
                        cmd.Parameters.AddWithValue("@category", txtCatogory.Text);
                        cmd.Parameters.AddWithValue("@name", txtName.Text);
                        cmd.Parameters.AddWithValue("@author", txtAuthor.Text);
                        cmd.Parameters.AddWithValue("@quantity", int.Parse(txtQuantity.Text));
                        cmd.Parameters.AddWithValue("@Adate", DateTime.Now);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Book details  successfully Updated.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            panel1.Visible = false;

                            gridLoad();
                        }
                        else
                        {
                            MessageBox.Show("Failed to Update book details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                
            }
            catch(Exception e1)
            {
                MessageBox.Show(""+e1, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    txtSearchBooks.Text = "";
                    txtSearchBooks.MaxLength = 13;
                    
                    break;
                case 1:
                    txtSearchBooks.Text = "";
                    txtSearchBooks.MaxLength = 50;

                    break;
                case 2:
                    txtSearchBooks.Text = "";
                    txtSearchBooks.MaxLength = 50;

                    break;
                case 3:
                    txtSearchBooks.Text = "";
                    txtSearchBooks.MaxLength = 50;

                    break;
                default:

                    break;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearchBooks.Text = string.Empty;
            panel1.Visible = false;
            pictureBox1.Visible = true;
            pictureBox2.Visible = false;
            BackColor = Color.FromArgb(215, 242, 242);
            gridLoad();
        }

        private void txtSearchBooks_TextChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    searchquery = "SELECT * FROM NewBook WHERE b_isbn LIKE '%" + txtSearchBooks.Text + "%'";
                    break;
                case 1:
                    searchquery = "SELECT * FROM NewBook WHERE b_name LIKE '%" + txtSearchBooks.Text + "%'";
                    break;
                case 2:
                    searchquery = "SELECT * FROM NewBook WHERE b_author LIKE '%" + txtSearchBooks.Text + "%'";
                    break;
                case 3:
                    searchquery = "SELECT * FROM NewBook WHERE b_category LIKE '%" + txtSearchBooks.Text + "%'";
                    break;

                default: 
                    break;
            }

            if (txtSearchBooks.Text == "")
            {
                gridLoad();
            }
            else
            {
                using (SqlCommand cmd1 = new SqlCommand(searchquery, conn))
                {
                    try
                    {
                        conn.Open();
                        object result = cmd1.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            conn.Close();
                            SqlDataAdapter da2 = new SqlDataAdapter(cmd1);
                            DataSet ds2 = new DataSet();
                            da2.Fill(ds2);
                            dataGridView1.DataSource = ds2.Tables[0];
                        }
                        else
                        {
                            conn.Close();
                            SqlDataAdapter da2 = new SqlDataAdapter(cmd1);
                            DataSet ds2 = new DataSet();
                            da2.Fill(ds2);
                            dataGridView1.DataSource = ds2.Tables[0];
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(" " + ex);
                    }
                }
            }
        }

        private void txtSearchBooks_Click(object sender, EventArgs e)
        {
            pictureBox2.Visible = true;
            BackColor = Color.White;
            pictureBox1.Visible = false;
        }
    }
}
