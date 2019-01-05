using System;
using System.Collections.Generic;

namespace DataModel.Models.User
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string DeviceId { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public ICollection<UserMetadata> Metadata { get; set; }
        public ICollection<UserAnswer> UserAnswers { get; set; }
    }
}
