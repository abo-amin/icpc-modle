using System.Collections.Generic;

namespace icpc_modle.Models
{
    public class ExamViewModel
    {

        public int QuestionNumber { get; set; }
        public string QuestionText { get; set; }
        public List<string> Choices { get; set; }
        public string QuestionImageUrl { get; set; }
        public int TimerSeconds { get; set; }

        public ExamViewModel() { }
        public string UserName { get; set; } // ✅ تأكد من وجود هذه الخاصية

        public DateTime StartTime { get; set; }// ⏳ وقت بدء الامتحان
       
            public AllowedEmail AllowedEmail { get; set; }
            // Other properties
        

    }
}
