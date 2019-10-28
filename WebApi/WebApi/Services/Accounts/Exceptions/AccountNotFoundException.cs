using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Services.Accounts.Exceptions
{
    public class AccountNotFoundException:ApplicationException
    {
        public AccountNotFoundException() : base(null)
        { }

        public AccountNotFoundException(string message) : base(message) { }
    }
}
