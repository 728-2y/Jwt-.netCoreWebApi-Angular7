using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Services.Accounts.Exceptions
{
    public class DuplicateRoleException:ApplicationException
    {
        public DuplicateRoleException() : base(null) { }

        public DuplicateRoleException(string message) : base(message) { }
    }
}
