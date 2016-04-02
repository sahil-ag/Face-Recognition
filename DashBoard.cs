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
            user.Read();
            Console.WriteLine(user.GetInt32(0));
            //label6.Text = "ID : " + user.GetInt32(0).ToString();
            //label7.Text = "Name : " + user.GetString(1);
            InitializeComponent();
        }

        

        

        private void label1_Click(object sender, EventArgs e)
        {
            new CameraCapture().Show();
            this.Hide();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            new Recogniser().Show();
            this.Hide();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            new Privilege().Show();
            this.Hide();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            new guardControl().Show();
            this.Hide();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
