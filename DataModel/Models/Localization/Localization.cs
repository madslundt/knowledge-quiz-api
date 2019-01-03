using System;

namespace DataModel.Models
{
    public class Localization
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Updated { get; set; }

        public string Key { get; set; }
        public string Translation { get; set; }
        public Locale Locale { get; set; }

        public QuestionLocalization QuestionLocalization { get; set; }
        public AnswerLocalization AnswerLocalization { get; set; }
    }

    public enum Locale
    {
        en_US = 1,
        da_DK = 2
    }
}
