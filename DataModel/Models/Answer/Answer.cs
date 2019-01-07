using System;
using System.Collections.Generic;
using DataModel.Models.User;

namespace DataModel.Models.Answer
{
    public class Answer
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; }

        public Guid QuestionId { get; set; }
        public Question.Question Question { get; set; }

        public bool IsCorrect { get; set; } = false;

        public ICollection<AnswerLocalization> AnswerLocalizations { get; set; }
        public ICollection<UserAnswer> UserAnswers { get; set; }
    }
}
