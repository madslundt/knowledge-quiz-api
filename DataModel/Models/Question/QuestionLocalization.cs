using System;

namespace DataModel.Models.Question
{
    public class QuestionLocalization
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid QuestionId { get; set; }
        public Question Question { get; set; }

        public Guid TranslationId { get; set; }
        public Localization.Localization Localization { get; set; }

        public QuestionType QuestionType { get; set; }
        public QuestionTypeReference QuestionTypeReference { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
