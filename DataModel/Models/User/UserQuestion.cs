using System;

namespace DataModel.Models.User
{
    public class UserQuestion
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid QuestionId { get; set; }
        public Question.Question Question { get; set; }

        public bool HintUsed { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
