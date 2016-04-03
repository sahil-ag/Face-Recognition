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
    public partial class setting : Form
    {
        int userId;
        public setting(SQLiteDataReader ans)
        {
            ans.Read();
            userId = ans.GetInt32(0);

            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        
        private void btnRecognise_Click_1(object sender, EventArgs e)
        {
            if (textBox1.Text != null)
            {
                SQLiteConnection _sqLiteConnection;
                _sqLiteConnection = new SQLiteConnection(String.Format("Data Source=face.sqlite;Version=3;"));

                try
                {
                    _sqLiteConnection.Open();
                    var selectQuery = "UPDATE guard SET password = @pass WHERE  id = @userId";
                    var cmd = new SQLiteCommand(selectQuery, _sqLiteConnection);
                    cmd.Parameters.AddWithValue("userId", userId);
                    cmd.Parameters.AddWithValue("pass", textBox1.Text);
                    var result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Updated!");
                        textBox1.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Error! Contact Support.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    _sqLiteConnection.Close();
                }

            }
        }
    }
}
