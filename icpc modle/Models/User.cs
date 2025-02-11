
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace icpc_modle.Models
{
    public class UserAnswer
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public int QuestionId { get; set; }
        public string Answer { get; set; }
        public Question Question { get; set; }
        public DateTime? StartTime { get; set; } // ⏳ وقت بدء السؤال

    }
}