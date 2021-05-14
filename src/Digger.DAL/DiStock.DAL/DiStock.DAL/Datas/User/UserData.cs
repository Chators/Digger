namespace DiStock.DAL
{
    public class UserData
    {
        public int Id { get; set; }

        public string Pseudo { get; set; }

        public byte[] Password { get; set; }

        public string Role { get; set; }
    }
}
