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
    public partial class AddUser : Form
    {
        public AddUser()
        {
            InitializeComponent();
        }
        SqlConnection conn = new SqlConnection("Data Source=KAVISHKAJANITH\\SQLEXPRESS;Initial Catalog=LibraryManagmentSystem;Integrated Security=True");

        private void AddUser_Load(object sender, EventArgs e)
        {
            LastUser();
        }
        private void LastUser()
        {
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 L_id FROM LibraryUsers ORDER BY L_id DESC;", conn))
            {
                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    string name = result.ToString();
                    lblLastID.Text = name;
                }
                else
                {
                    lblLastID.Text = "This is first User";
                }
                conn.Close();
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtLibraryID.Text != "" && txtName.Text != "" && txtAddress.Text != "" && txtPhone.Text != "" && txtEmail.Text != "")
            {
                try
                {
                    string query = "INSERT INTO LibraryUsers(L_id,L_name,L_address,L_phone,L_email,L_membership_Update) VALUES(@Lid,@Lname,@Laddress,@Lphone,@Lemail,@Ldate)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@Lid", txtLibraryID.Text);
                        cmd.Parameters.AddWithValue("@Lname", txtName.Text);
                        cmd.Parameters.AddWithValue("@Laddress", txtAddress.Text);
                        cmd.Parameters.AddWithValue("@Lphone", txtPhone.Text);
                        cmd.Parameters.AddWithValue("@Lemail", txtEmail.Text);
                        cmd.Parameters.AddWithValue("@Ldate", DateTime.Now);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            conn.Close();
                            LastUser();
                            MessageBox.Show("User added successfully.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtLibraryID.Clear();
                            txtName.Clear();
                            txtAddress.Clear();
                            txtPhone.Clear();
                            txtEmail.Clear();
                        }
                        else
                        {
                            MessageBox.Show("Failed to add User.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Input Fields Not in Correct Format", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please Fill the All the Details.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are You Sure,This Will Delete Your Unsaved Data", "WARNING", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                this.Close();
            }
        }
    }
}
