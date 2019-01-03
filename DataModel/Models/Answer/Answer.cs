using System;

namespace DataModel.Models
{
    public class Answer
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Updated { get; set; }

        public AnswerLocalization AnswerLocalization { get; set; }

        public Question Question { get; set; }
    }
}
