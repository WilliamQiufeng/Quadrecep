using Godot;
using SQLite;

namespace Quadrecep.Database
{
    public class DatabaseHandler
    {
        public static readonly string DatabasePath = OS.GetUserDataDir() + "/game.db";
        public static SQLiteConnection Connection { get; private set; }
    
        public static void Initialize() {
            Connection = new SQLiteConnection(DatabasePath);
            Connection.CreateTable<MapRecord>();
        }
    }
}
