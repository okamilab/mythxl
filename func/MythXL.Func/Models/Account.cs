namespace MythXL.Func.Models
{
    public class Account
    {
        public string Address { get; private set; }

        public string Password { get; private set; }

        internal Account(string address, string password)
        {
            Address = address;
            Password = password;
        }
    }
}
