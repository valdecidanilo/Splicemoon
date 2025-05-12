using SQLite;

namespace DB.Data
{
    public class UserData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public string Email { get; set; }

        public string PasswordHash { get; set; }
        public string Salt { get; set; }

        public string Nickname { get; set; }
    }
}