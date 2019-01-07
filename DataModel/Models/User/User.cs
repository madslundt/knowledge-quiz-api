﻿using System;
using System.Collections.Generic;

namespace DataModel.Models.User
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UniqueId { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public UserBlacklist UserBlacklist { get; set; }

        public ICollection<UserMetadata> Metadata { get; set; }
        public ICollection<UserAnswer> UserAnswers { get; set; }
        public ICollection<UserQuestion> UserQuestions { get; set; }
    }
}
