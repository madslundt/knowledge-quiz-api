using System;

namespace DataModel.Models.User
{
    public class UserMetadata
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public UserMetadataTypeReference MetadataTypeReference { get; set; }
        public UserMetadataType MetadataType { get; set; }

        public string Value { get; set; }

        public DateTime Created { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
