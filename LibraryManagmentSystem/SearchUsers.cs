using System;
using System.Collections;
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
    public partial class ViewUsers : Form
    {
        public ViewUsers()
        {
            InitializeComponent();
        }
        SqlConnection conn = new SqlConnection("Data Source=KAVISHKAJANITH\\SQLEXPRESS;Initial Catalog=LibraryManagmentSystem;Integrated Security=True");

        private void gridLoad()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM LibraryUsers ", conn);
            SqlDataAdapter da1 = new SqlDataAdapter(cmd);
            DataSet ds1 = new DataSet();
            da1.Fill(ds1);

            dataGridView1.DataSource = ds1.Tables[0];
        }
        private int index;
        
        private void ViewUsers_Load(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;
            panel2.Visible = false;
            gridLoad();
            index = comboBox1.SelectedIndex = 0;

            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.YellowGreen;
            dataGridView1.RowsDefaultCellStyle.BackColor = Color.YellowGreen;

            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige;

            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Green;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Yellow;

            

            dataGridView1.Columns[0].HeaderText = "Library ID";
            dataGridView1.Columns[1].HeaderText = "Name";
            dataGridView1.Columns[2].HeaderText = "Address";
            dataGridView1.Columns[3].HeaderText = "Phone NO.";
            dataGridView1.Columns[4].HeaderText = "Email";
            dataGridView1.Columns[5].HeaderText = "Membership Renew Date";
            dataGridView1.Columns[0].Width = 100;
            dataGridView1.Columns[1].Width = 200;
            dataGridView1.Columns[2].Width = 200;
            dataGridView1.Columns[3].Width = 107;
            dataGridView1.Columns[4].Width = 100;
            dataGridView1.Columns[5].Width = 100;

            
            
        }

        private void Refresh()
        {
            panel2.Visible = false;
            gridLoad();
            txtSearchUser.Text = string.Empty;
            pictureBox1.Visible = false;
            BackColor = Color.FromArgb(226, 255, 255);
        }

        private void txtSearchUser_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;
            BackColor = Color.White;

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        private string clicked_L_ID;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                
                clicked_L_ID = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                panel2.Visible = true;
                string query = "SELECT * FROM LibraryUsers WHERE L_id = @L_ID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@L_ID", clicked_L_ID);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        lblLID.Text = ds.Tables[0].Rows[0][0].ToString();
                        txtName.Text = ds.Tables[0].Rows[0][1].ToString();
                        txtAddress.Text = ds.Tables[0].Rows[0][2].ToString();
                        txtPhone.Text = ds.Tables[0].Rows[0][3].ToString();
                        txtEmail.Text = ds.Tables[0].Rows[0][4].ToString();
                    }
                    conn.Close();
                }
            }
        }
        
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Data Will Be Updated,Confirm?", "Succsusfull", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    string query = "UPDATE LibraryUsers SET L_name = @Lname, L_address = @Laddress, L_phone = @Lphone, L_email = @Lemail,L_membership_Update = @Ldate WHERE L_id = @Lid";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@Lid", clicked_L_ID);
                        cmd.Parameters.AddWithValue("@Lname", txtName.Text);
                        cmd.Parameters.AddWithValue("@Laddress", txtAddress.Text);
                        cmd.Parameters.AddWithValue("@Lphone", txtPhone.Text);
                        cmd.Parameters.AddWithValue("@Lemail", txtEmail.Text);
                        cmd.Parameters.AddWithValue("@Ldate", DateTime.Now);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("User details  successfully Updated.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Refresh();
                        }
                        else
                        {
                            MessageBox.Show("Failed to Update User details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

            }
            catch (Exception e1)
            {
                MessageBox.Show("" + e1, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Data Will Be Deleted,Confirm?", "Succsusfull", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    string query = "DELETE FROM LibraryUsers WHERE L_id = @L_ID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@L_ID", clicked_L_ID);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            conn.Close();
                            MessageBox.Show("User details  successfully Deleted.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Refresh();
                        }
                        else
                        {
                            MessageBox.Show("Failed to Delete User details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch
            {

            }
        }
        
        private string searchquery;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    txtSearchUser.MaxLength = 8;
                    txtSearchUser.Text = "";

                    break;
                case 1:
                    txtSearchUser.MaxLength = 20;
                    txtSearchUser.Text = "";

                    break;
                default:

                    break;
            }
        }
        private void txtSearchUser_TextChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                searchquery = "SELECT * FROM LibraryUsers WHERE L_id LIKE '%" + txtSearchUser.Text + "%'";
            }
            else
            {
                searchquery = "SELECT * FROM LibraryUsers WHERE L_name LIKE '%" + txtSearchUser.Text + "%'";
            }


            if (txtSearchUser.Text=="")
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
                            conn.Close() ;
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
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if ((MessageBox.Show("Are You Sure, This Will Delete Your Unsaved Data", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)==DialogResult.OK))
            {
                Refresh();
            }
        }
    }
}
