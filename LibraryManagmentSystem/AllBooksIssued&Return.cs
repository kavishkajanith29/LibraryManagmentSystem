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

namespace LibraryManagmentSystem
{
    public partial class BooksDetails : Form
    {
        public BooksDetails()
        {
            InitializeComponent();
        }

        SqlConnection conn = new SqlConnection("Data Source=KAVISHKAJANITH\\SQLEXPRESS;Initial Catalog=LibraryManagmentSystem;Integrated Security=True");
        private void gridLoad()
        {
            conn.Open();
            comboBox1.SelectedIndex = 0;
            SqlCommand cmd = new SqlCommand("SELECT * FROM IssueBooks ORDER  BY isuue_date DESC", conn);
            SqlDataAdapter da1 = new SqlDataAdapter(cmd);
            DataSet ds1 = new DataSet();
            da1.Fill(ds1);

            dataGridView1.DataSource = ds1.Tables[0];
            conn.Close();
        }
        private string searchquery;
        private void BooksDetails_Load(object sender, EventArgs e)
        {
            gridLoad();
            dataGridView1.Columns[0].HeaderText = "LIBRARY ID";
            dataGridView1.Columns[1].HeaderText = "BOOK ISBN";
            dataGridView1.Columns[2].HeaderText = "ISSUE DATE";
            dataGridView1.Columns[3].HeaderText = "RETURN DATE";
            dataGridView1.Columns[4].HeaderText = "LATE FEE";
            dataGridView1.Columns[0].Width = 150;
            dataGridView1.Columns[1].Width = 150;
            dataGridView1.Columns[2].Width = 200;
            dataGridView1.Columns[3].Width = 200;
            dataGridView1.Columns[4].Width = 107;



            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.YellowGreen;
            dataGridView1.RowsDefaultCellStyle.BackColor = Color.YellowGreen;

            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige;

            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Green;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Yellow;

        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    txtSearch.Text = "";
                    txtSearch.MaxLength = 50;


                    break;
                case 1:
                    txtSearch.Text = "";
                    txtSearch.MaxLength = 13;

                    break;
                default:

                    break;
            }
        }

        private void txtSearchBooks_TextChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    searchquery = "SELECT * FROM IssueBooks WHERE L_id LIKE '%" + txtSearch.Text + "%'";
                    break;
                case 1:
                    searchquery = "SELECT * FROM IssueBooks WHERE b_isbn LIKE '%" + txtSearch.Text + "%'";
                    break;
                default:
                    break;
            }

            if (txtSearch.Text == "")
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Enabled = false;
            comboBox1.SelectedIndex = 0;
            txtSearch.Text = string.Empty;
            gridLoad();
        }
    }
}
