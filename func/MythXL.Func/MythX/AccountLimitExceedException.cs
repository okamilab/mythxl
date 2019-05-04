using System;

namespace MythXL.Func
{
    public class AccountLimitExceedException : Exception
    {
        public string Address { get; private set; }

        public AccountLimitExceedException(string address)
        {
            Address = address;
        }
    }
}
