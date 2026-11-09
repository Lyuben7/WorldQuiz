namespace WebApplication1.Models
{
    public class Quiz
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public List<QuizQuestion> Questions { get; set; } = new();
    }

    public class QuizQuestion
    {
        public string Text { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
        public int CorrectIndex { get; set; }
    }
}
