using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class JwtTokenSetting
    {
        public string Issuer { get; set; }
        public string SecretKey { get; set; }
    }
}
