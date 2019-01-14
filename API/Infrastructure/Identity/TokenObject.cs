using System;

namespace API.Infrastructure.Identity
{
    public class TokenObject
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
