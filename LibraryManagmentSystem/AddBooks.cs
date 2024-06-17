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
using System.Xml.Linq;

namespace LibraryManagmentSystem
{
    public partial class AddBooks : Form
    {
        public AddBooks()
        {
            InitializeComponent();
        }

        SqlConnection conn = new SqlConnection("Data Source=KAVISHKAJANITH\\SQLEXPRESS;Initial Catalog=LibraryManagmentSystem;Integrated Security=True");
        private void BookCategory()
        {
            comboBox1.SelectedIndex = -1;
            conn.Open();
            string query = "SELECT DISTINCT b_category FROM NewBook";
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string item = reader["b_category"].ToString();
                        comboBox1.Items.Add(item);
                    }
                    conn.Close();
                }
            }
        }
        private void AddBooks_Load(object sender, EventArgs e)
        {
            txtCatogory.Visible = false;
            BookCategory();
        }

        private int clickCount;
        private void btnNewCategory_Click(object sender, EventArgs e)
        {
            clickCount++;

            if (clickCount % 2 == 1)
            {
                txtCatogory.Visible = true;
                comboBox1.Enabled = false;
                comboBox1.Visible = false;
            }
            else
            {
                txtCatogory.Visible = false;
                comboBox1.Enabled = true;
                comboBox1.Visible = true;
            }
        }

        private string category;


        private void btnSave_Click(object sender, EventArgs e)
        {
            if (comboBox1.Enabled)
            {
                category = comboBox1.SelectedItem.ToString();
            }
            else
            {
                category = txtCatogory.Text;
            }
            if ((txtISBN.Text!=""&& txtBookName.Text != "" && txtAuthor.Text != "" && txtQuantity.Text != "")&&(txtCatogory.Text!=""|| comboBox1.SelectedItem != null))
            {
                try
                {
                    string query = "INSERT INTO NewBook(b_isbn,b_category,b_name,b_author,b_quantity,b_updated_date) VALUES(@ISBN,@category,@name,@author,@quantity,@Adate)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@ISBN", txtISBN.Text);
                        cmd.Parameters.AddWithValue("@category", category);
                        cmd.Parameters.AddWithValue("@name", txtBookName.Text);
                        cmd.Parameters.AddWithValue("@author", txtAuthor.Text);
                        cmd.Parameters.AddWithValue("@quantity", int.Parse(txtQuantity.Text));
                        cmd.Parameters.AddWithValue("@Adate", DateTime.Now);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            conn.Close();
                            MessageBox.Show("The book was added successfully.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtISBN.Clear();
                            txtBookName.Clear();
                            txtAuthor.Clear();
                            txtQuantity.Clear();
                            txtCatogory.Clear();
                            txtCatogory.Visible = false;
                            comboBox1.Enabled = true;
                            comboBox1.Visible = true;
                            comboBox1.Items.Clear();
                            BookCategory();
                        }
                        else
                        {
                            conn.Close();
                            MessageBox.Show("Failed to add book.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception e2)
                {
                    MessageBox.Show(e2+"Input Fields Not in Correct Format", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please Fill The All The Details", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are You Sure,This Will Delete Your Unsaved Data", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                this.Close();

            }
        }
    }
}
