using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hack_in_the_north_hand_mouse
{
    public partial class LoginForm : Form
    {
        private SQLiteConnection sqLiteConnection;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void UserName_Enter(Object sender, EventArgs e)
        {
            UserName.Text = null;
        }
        private void UserName_TextChanged(object sender, EventArgs e)
        {
            UserName.ForeColor = Color.Black;       
        }

        private void Password_TextChanged(object sender, EventArgs e)
        {
            Password.ForeColor = Color.Black;
        }

        private void Password_Enter(Object sender, EventArgs e)
        {
            Password.Text = null;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            sqLiteConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", "face.sqlite"));
            //dbclass.make_connection(sqLiteConnection, "guard.sqlite");
            try
            {
                sqLiteConnection.Open();
                string username = UserName.Text;
                string password = Password.Text;

                var Query = "SELECT * from guard where username=@username AND password=@password ";

                //MessageBox.Show(username + " " + password + "///" + Query);

                var cmd = new SQLiteCommand(Query, sqLiteConnection);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", password);

                var result = cmd.ExecuteReader();

                if (result.HasRows)
                {
                    MessageBox.Show("Welcome " + username);
                    new DashBoard(result).Show();
                    this.Visible = false;

                }
                else
                {
                    MessageBox.Show("Sorry, Please check your username or password");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
