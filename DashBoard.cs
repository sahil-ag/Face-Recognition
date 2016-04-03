using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Hack_in_the_north_hand_mouse
{
    public partial class DashBoard : Form
    {
        private SQLiteDataReader user;
        public DashBoard(SQLiteDataReader ans)
        {
            user = ans;
            //user.Read();
           // Console.WriteLine(user.GetInt32(0));
           
           // var a = user.GetInt32(0);
            //label6.Text = a.ToString();
            //label6.Text = "ID : " + user.GetInt32(0).ToString();
            //label7.Text = "Name : " + user.GetString(1);
            InitializeComponent();
        }

        

 
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void btnAddData_Click(object sender, EventArgs e)
        {
            new Detector().Show();
            this.Hide();
        }

                private void btnRecognise_Click_1(object sender, EventArgs e)
        {
            new Recogniser().Show();
            this.Hide();
        }

        private void btnSettings_Click_1(object sender, EventArgs e)
        {
            new setting(user).Show();
            this.Hide();
        }
    }
}
