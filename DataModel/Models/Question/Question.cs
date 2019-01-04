using System;
using System.Collections.Generic;

namespace DataModel.Models.Question
{
    public class Question
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Updated { get; set; }
        
        public string ImageUrl { get; set; }

        public ICollection<QuestionLocalization> QuestionTranslations { get; set; }
        public ICollection<Answer.Answer> Answers { get; set; }
    }
}
