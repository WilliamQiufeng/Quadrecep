using SQLite;

namespace Quadrecep.Database
{
    [Table("Maps")]
    public class MapRecord
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        [Indexed]
        public int Id { get; set; }

        [Column("name")] public string Name { get; set; }
        [Column("creator")] public string Creator { get; set; }
        [Column("artist")] public string Artist { get; set; }

        [Column("audio_path")] public string AudioPath { get; set; }
        [Column("background_path")] public string BackgroundPath { get; set; }
    }
}