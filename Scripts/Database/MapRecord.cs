using SQLite;
namespace Quadrecep.Scripts.Database
{
    [Table("Maps")]
    public class MapRecord
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        [Indexed]
        public int Id {get; set; }
        [Column("name")]
        public string Name {get; set; }
    }
}