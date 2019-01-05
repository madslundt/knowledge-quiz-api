using System.Collections.Generic;
using DataModel.Models.Question;

namespace DataModel.Models
{
    public class QuestionTypeReference
    {
        public QuestionType Id { get; set; }
        public string Name { get; set; }

        public ICollection<QuestionLocalization> QuestionLocalizations { get; set; }
    }

    public enum QuestionType
    {
        Question = 1,
        Hint = 2,
        Result = 3
    }
}
