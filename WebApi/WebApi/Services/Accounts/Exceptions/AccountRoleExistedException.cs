using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Services.Accounts.Exceptions
{
    public class AccountRoleExistedException:ApplicationException
    {
        public AccountRoleExistedException() : base(null) { }
        public AccountRoleExistedException(string message) : base(message) { }
    }
}
