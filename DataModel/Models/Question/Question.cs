using System;
using System.Collections.Generic;

namespace DataModel.Models
{
    public class Question
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Updated { get; set; }

        public QuestionLocalization QuestionLocalization { get; set; }
        public string ImageUrl { get; set; }

        public ICollection<Answer> Answers { get; set; }
    }
}
