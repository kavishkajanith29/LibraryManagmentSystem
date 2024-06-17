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
    public partial class EmpDashboard : Form
    {
        private string empID;
        SqlConnection conn = new SqlConnection("Data Source=KAVISHKAJANITH\\SQLEXPRESS;Initial Catalog=LibraryManagmentSystem;Integrated Security=True");
        public EmpDashboard()
        {
            InitializeComponent();
        }

        private void EmpDashboard_Load(object sender, EventArgs e)
        {
            panelData.Visible = false;
            NameAndID();
        }

        private void NameAndID()
        {
            conn.Open();
            string query = "SELECT el.E_id, e.E_name FROM EmployeeLogin el, Employee e WHERE el.E_id = e.E_id AND el.E_userName = @Username";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Username", LogIn.user_name);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblempID.Text = reader["E_id"].ToString();
                    lblName.Text = reader["E_name"].ToString();
                }

                reader.Close();
                conn.Close();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void loadform(object Form)
        {
            if (this.panelData.Controls.Count > 0)
                this.panelData.Controls.RemoveAt(0);
            Form f = Form as Form;
            f.TopLevel = false;
            f.Dock = DockStyle.Fill;
            this.panelData.Controls.Add(f);
            this.panelData.Tag = f;
            f.Show();
        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IssueBooks.captureDevice != null && IssueBooks.captureDevice.IsRunning)
            {
                IssueBooks.captureDevice.Stop();
            }
            if (ReturnBooks.captureDevice != null && ReturnBooks.captureDevice.IsRunning)
            {
                ReturnBooks.captureDevice.Stop();
            }
            this.Close(); 
        }

        private void addBooksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelData.Visible = Visible;
            loadform(new AddBooks());
        }

        private void searchBooksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelData.Visible = Visible;
            loadform(new SearchBooks());
        }

        private void addUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelData.Visible = Visible;
            loadform(new AddUser());
        }

        private void viewUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelData.Visible = Visible;
            loadform(new ViewUsers());
        }

        private void allBooksDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelData.Visible = Visible;
            loadform(new BooksDetails());
        }

        private void isuueBooksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelData.Visible = Visible;
            loadform(new IssueBooks());
        }

        private void retunBooksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelData.Visible = Visible;
            loadform(new ReturnBooks());
        }
        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelData.Visible=false;
        }
    }
}
