using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel.Models.User
{
    public class UserBlacklist
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
