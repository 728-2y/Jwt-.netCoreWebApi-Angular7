using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Services.Accounts.Exceptions
{
    public class RoleNotFoundException:ApplicationException
    {
        public RoleNotFoundException() : base(null) { }

        public RoleNotFoundException(string message) : base(message) { }
    }
}
