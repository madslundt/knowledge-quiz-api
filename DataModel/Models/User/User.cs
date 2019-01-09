using System;
using System.Collections.Generic;
using DataModel.Models.Question;

namespace DataModel.Models.User
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UniqueId { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public ICollection<UserMetadata> Metadatas { get; set; }
        public ICollection<UserAnswer> UserAnswers { get; set; }
        public ICollection<UserQuestion> UserQuestions { get; set; }
        public ICollection<QuestionReport> QuestionReports { get; set; }
    }
}
