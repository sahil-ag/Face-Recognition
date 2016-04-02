using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack_in_the_north_hand_mouse
{
    class database_class
    {
        public void make_connection( SQLiteConnection connection, String path )
        {
            connection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", path));
        }
        public void close_connection(SQLiteConnection connection)
        {
            connection.Close();
        }
    }
}
