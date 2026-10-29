namespace WebApplication1.ViewModels
{
    public class QuizPlayViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public List<QuizPlayQuestionViewModel> Questions { get; set; } = new();
    }

    public class QuizPlayQuestionViewModel
    {
        public int Index { get; set; }
        public string Text { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
    }

    public class QuizSubmitViewModel
    {
        public string Id { get; set; } = string.Empty;
        public List<int> QuestionOrder { get; set; } = new();
        public List<int?> Answers { get; set; } = new();
    }

    public class QuizResultViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Score { get; set; }
        public int Total { get; set; }
        public List<QuizResultItemViewModel> Items { get; set; } = new();
    }

    public class QuizResultItemViewModel
    {
        public string Text { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
        public int SelectedIndex { get; set; }
        public int CorrectIndex { get; set; }
        public bool IsCorrect { get; set; }
    }
}
