using System;

namespace DataModel.Models.Answer
{
    public class AnswerLocalization
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid AnswerId { get; set; }
        public Answer Answer { get; set; }

        public Guid LocalizationId { get; set; }
        public Localization.Localization Localization { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
