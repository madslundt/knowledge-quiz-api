using System;
using System.Collections.Generic;

namespace DataModel.Models.Answer
{
    public class Answer
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Updated { get; set; }

        public Guid QuestionId { get; set; }
        public Question.Question Question { get; set; }

        public ICollection<AnswerLocalization> AnswerLocalizations { get; set; }
    }
}
