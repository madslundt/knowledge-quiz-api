using System;
using System.Collections.Generic;

namespace DataModel.Models.Localization
{
    public class Localization
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Updated { get; set; }
        
        public string Text { get; set; }

        public Locale Locale { get; set; }
        public LocaleReference LocaleReference { get; set; }

        public ICollection<Question.QuestionLocalization> QuestionLocalizations { get; set; }
        public ICollection<Answer.AnswerLocalization> AnswerLocalizations { get; set; }
    }
}
