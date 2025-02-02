namespace icpc_modle.Models
{
    public class ExamAttempt
    {
        public string Email { get; set; }
        public List<AnswerRecord> Answers { get; set; } = new List<AnswerRecord>();

    }
}
