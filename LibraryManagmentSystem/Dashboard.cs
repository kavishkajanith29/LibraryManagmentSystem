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
    public partial class Dashboard : Form
    {
        private void Dashboard_Load(object sender, EventArgs e)
        {

        }
        public void loadform(object Form) {
            if (this.panelData.Controls.Count > 0)
                this.panelData.Controls.RemoveAt(0);
            Form f = Form as Form;
            f.TopLevel = false;
            f.Dock = DockStyle.Fill;
            this.panelData.Controls.Add(f);
            this.panelData.Tag = f;
            f.Show();
        }
        SqlConnection conn = new SqlConnection("Data Source=KAVISHKAJANITH\\SQLEXPRESS;Initial Catalog=LibraryManagmentSystem;Integrated Security=True");
        public Dashboard(String str)
        {
            InitializeComponent();
            using (SqlCommand cmd = new SqlCommand("SELECT name FROM library_users WHERE user_name =@username", conn))
            {
                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@username", str);
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        string name = result.ToString();
                        lblName.Text = name;
                    }
                    else
                    {

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

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            loadform(new ViewUsers());
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            loadform(new SearchBooks());
        }
    }
}