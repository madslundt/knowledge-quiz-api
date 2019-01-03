using System;

namespace DataModel.Models
{
    public class QuestionLocalization
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid QuestionId { get; set; }
        public Question Question { get; set; }

        public Guid LocalizationId { get; set; }
        public Localization Localization { get; set; }

        public QuestionType QuestionType { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;
    }

    public enum QuestionType
    {
        Question = 1,
        Hint = 2
    }
}
