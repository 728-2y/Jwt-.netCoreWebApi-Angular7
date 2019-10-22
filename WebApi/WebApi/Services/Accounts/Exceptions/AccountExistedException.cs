using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Services.Accounts.Exceptions
{
    public class AccountExistedException:ApplicationException
    {
        public AccountExistedException() : base(null) { }

        public AccountExistedException(string message) : base(message) { }
    }
}
