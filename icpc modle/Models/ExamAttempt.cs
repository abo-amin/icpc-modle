using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace icpc_modle.Models
{
    public class ExamAttempt
    {
        [Key]
        public int Id { get; set; } // ✅ إضافة ID لجعل المحاولة كيان مستقل
        public string Email { get; set; }

        // ✅ إضافة علاقة مع AnswerRecord وربطها بالمحاولة
        public List<AnswerRecord> Answers { get; set; } = new List<AnswerRecord>();
    }
}
