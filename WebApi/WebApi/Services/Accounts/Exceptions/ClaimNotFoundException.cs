using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Services.Accounts.Exceptions
{
    public class ClaimNotFoundException:ApplicationException
    {
        public ClaimNotFoundException() : base(null) { }

        public ClaimNotFoundException(string message) : base(message) { }
    }
}
