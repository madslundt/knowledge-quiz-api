using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel.Models.Question
{
    public class QuestionReport
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        public User.User User { get; set; }

        public Guid QuestionId { get; set; }
        public Question Question { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
