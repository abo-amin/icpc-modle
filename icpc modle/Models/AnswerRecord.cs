// Models/AnswerRecord.cs
namespace icpc_modle.Models
{
    public class AnswerRecord
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string SelectedAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public string QuestionImageUrl { get; set; }
        public bool IsCorrect { get; set; }
    }
}