using System.ComponentModel.DataAnnotations;

namespace icpc_modle.Models
{
    public class ExamResult
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public double Percentages { get; set; }
        public string Status { get; set; }
    }
}
