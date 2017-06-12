namespace App5
{
    public class User
    {
        public User(int idUser, string name, string surname, string login, string avatar, string phoneNumber, string email, int isOnline, string password)
        {
            this.idUser = idUser;
            this.name = name;
            this.surname = surname;
            this.login = login;
            this.avatar = avatar;
            this.phoneNumber = phoneNumber;
            this.email = email;
            this.isOnline = isOnline;
            this.password = password;
        }

        public int idUser { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string login { get; set; }
        public string avatar { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public int isOnline { get; set; }
        public string password { get; set; }

    }
}