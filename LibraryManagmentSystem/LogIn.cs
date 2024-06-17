using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections;
using BCrypt.Net;

namespace LibraryManagmentSystem
{
    public partial class LogIn : Form
    {
        public static string user_name;
        public LogIn()
        {
            InitializeComponent();
        }
        SqlConnection conn = new SqlConnection("Data Source=KAVISHKAJANITH\\SQLEXPRESS;Initial Catalog=LibraryManagmentSystem;Integrated Security=True");
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtUser_name.Text == "")
            {
                lblUser_name.Text = "*Please Enter User Name"; 
            }
            else if (txtPassword.Text == "")
            {
                lblPassword.Text = "*Please Enter Password";
            }
            else
            {
                using (SqlCommand cmd = new SqlCommand("SELECT E_paassword FROM EmployeeLogin WHERE E_userName =@username", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@username", txtUser_name.Text);
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            string HashCode = result.ToString();
                            if (BCrypt.Net.BCrypt.Verify(txtPassword.Text,HashCode))
                            {
                                user_name = txtUser_name.Text;
                                EmpDashboard form3 = new EmpDashboard();
                                form3.ShowDialog();
                                txtPassword.Clear();
                                lblUser_name.Text = "";
                                lblPassword.Text = "";
                            }
                            else
                            {
                                lblPassword.Text = "*Invalid password";
                                txtPassword.Clear();
                            }

                        }
                        else
                        {
                            lblUser_name.Text = "*Invalid User Name";
                            lblPassword.Text = "*Invalid Password";
                            txtUser_name.Clear();
                            txtPassword.Clear();
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
        }
        private void txtPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Registration form2 = new Registration();
            form2.ShowDialog();
        }
    }
}