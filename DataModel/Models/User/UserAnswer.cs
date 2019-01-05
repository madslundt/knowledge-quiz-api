using System;

namespace DataModel.Models.User
{
    public class UserAnswer
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid AnswerId { get; set; }
        public Answer.Answer Answer { get; set; }

        public DateTime Answered { get; set; } = DateTime.UtcNow;
    }
}
