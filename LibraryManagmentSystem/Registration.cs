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
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace LibraryManagmentSystem
{
    public partial class Registration : Form
    {
        SqlConnection conn = new SqlConnection("Data Source=KAVISHKAJANITH\\SQLEXPRESS;Initial Catalog=LibraryManagmentSystem;Integrated Security=True");

        public Registration()
        {
            InitializeComponent();

            txtPassword1.TextChanged += txtPassword1_TextChanged;
            txtEmpID.TextChanged += txtLibraryID_TextChanged;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if (MessageBox.Show("Are You Sure,This will Delete Your Unsaved Data!", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                formClear();
                this.Close();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are You Sure,This will Delete Your Unsaved Data!", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                formClear();
            }
        }

        private void formClear()
        {
            txtEmpID.Clear();
            txtUser_name.Clear();
            txtPassword1.Clear();
            txtPassword2.Clear();
            lblEmpID.Text = "";
            lblUserName.Text = "";
            lblPassword1.Text = "";
            lblPassword2.Text = "";
            pictureBoxid.Visible = false;
            pictureBoxName.Visible = false;
            pictureBoxpass.Visible = false;
            pictureBoxcpass.Visible = false;
        }

        private void txtPassword1_TextChanged(object sender, EventArgs e)
        {
            if (txtEmpID.Text != "" && txtUser_name.Text != "")
            {
                string passwordPattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@#$%^&+=!]).{8,}$";

                bool isPasswordValid = Regex.IsMatch(txtPassword1.Text, passwordPattern);
                if (isPasswordValid)
                {
                    pictureBoxpass.Visible = true;
                    lblPassword1.Text = "";
                }
                else
                {
                    pictureBoxpass.Visible = false;
                    lblPassword1.Text = "* Your Password Should contain atleast 8 characters long,while containing one \nuppercase letter,one lowercase letter, one numbers and one special character";
                }
            }
            else if (txtEmpID.Text == "" && txtUser_name.Text == "")
            {
                lblEmpID.Text = "* Please Fill Your ID First";
                lblUserName.Text = "* Enter New User name First";
            }
            else if (txtEmpID.Text=="")
            {
                lblEmpID.Text = "* Please Fill Your ID First";
            }
            else if (txtUser_name.Text =="")
            {
                lblUserName.Text = "* Enter New User name First";
            }
        }
        private void txtLibraryID_TextChanged(object sender, EventArgs e)
        {
            LidCheck();
        }

        private void LidCheck()
        {
            try
            {
                using (SqlCommand cmd1 = new SqlCommand("SELECT E_id FROM Employee WHERE E_id =@EID", conn))
                {
                    conn.Open();
                    cmd1.Parameters.AddWithValue("@EID", txtEmpID.Text);
                    object result = cmd1.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        conn.Close();
                        lblEmpID.Text = "";
                        RegisterCheck();
                    }
                    else
                    {
                        conn.Close();
                        lblEmpID.Text = "* User Not Found.";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(" " + ex);
            }
        }

        private void RegisterCheck()
        {
            try
            {
                using (SqlCommand cmd1 = new SqlCommand("SELECT E_id FROM EmployeeLogin WHERE E_id =@EID", conn))
                {
                    conn.Open();
                    cmd1.Parameters.AddWithValue("@EID", txtEmpID.Text);
                    object result = cmd1.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        pictureBoxid.Visible = false;
                        lblEmpID.Text = "* User Already Registered";
                    }
                    else
                    {
                        pictureBoxid.Visible = true;
                        lblEmpID.Text = "";
                    }
                    conn.Close();
                }
            }
            catch
            {

            }
        }

        private void txtUser_name_TextChanged(object sender, EventArgs e)
        {
            UserNameCheck();
        }

        private void UserNameCheck()
        {
            if (txtEmpID.Text != "")
            {
                try
                {
                    using (SqlCommand cmd1 = new SqlCommand("SELECT E_userName FROM EmployeeLogin WHERE E_userName =@EName", conn))
                    {
                        conn.Open();
                        cmd1.Parameters.AddWithValue("@EName", txtUser_name.Text);
                        object result = cmd1.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            conn.Close();
                            pictureBoxName.Visible = false;
                            lblUserName.Text = "* User Name Already Exists.";
                        }
                        else
                        {
                            pictureBoxName.Visible = true;
                            lblUserName.Text = "";
                        }
                        conn.Close();
                    }
                }
                catch(Exception e1)
                {
                    MessageBox.Show(""+e1);
                }
            }
            else
            {
                lblEmpID.Text = "* Please Fill Your ID First.";
            }
        }
        
        private void btnConfirm_Click(object sender, EventArgs e)
        {

            if (txtEmpID.Text == "" || txtUser_name.Text == "" || txtPassword1.Text == ""  || txtPassword2.Text == "")
            {
                MessageBox.Show("Please Fill The All The Details!","Information",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            
            }
            else if(txtPassword1.Text==txtPassword2.Text)
            {
                try
                {
                    conn.Open();

                    string EmpID = txtEmpID.Text;
                    string user_name = txtUser_name.Text;
                    string password = txtPassword2.Text;
                    using (SqlCommand cmd2 = new SqlCommand("INSERT INTO EmployeeLogin(E_id,E_userName,E_paassword) VALUES(@EmpID,@user_name,@password)", conn))
                    {
                        cmd2.Parameters.AddWithValue("@EmpID", EmpID);
                        cmd2.Parameters.AddWithValue("@user_name", user_name);
                        cmd2.Parameters.AddWithValue("@password", BCrypt.Net.BCrypt.HashPassword(password));

                        int rowsAffected = cmd2.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Login data inserted successfully.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Registration Unsuccessful.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtEmpID.Text = string.Empty;
                            txtUser_name.Text = string.Empty;
                            txtPassword1.Text = string.Empty;
                            txtPassword2.Text = string.Empty;
                        }
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

        private void txtPassword2_TextChanged(object sender, EventArgs e)
        {
            if (txtPassword1.Text != txtPassword2.Text)
            {
                lblPassword2.Text = "* Password doesn't match";
            }
            else
            {
                pictureBoxcpass.Visible = true;
                lblPassword2.Text = "";
            }
        }
    }
}