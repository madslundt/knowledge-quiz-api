﻿using System;

namespace DataModel.Models.User
{
    public class UserMetadata
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public MetadataTypeReference MetadataTypeReference { get; set; }
        public UserMetadataType MetadataType { get; set; }

        public string Value { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
