namespace DataModel.Models.Question
{
    public class QuestionTypeReference
    {
        public QuestionType Id { get; set; }
        public string Name { get; set; }
    }

    public enum QuestionType
    {
        Question = 1,
        Hint = 2,
        Final = 3
    }
}
