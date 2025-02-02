namespace icpc_modle.Models
{
    public class ExamViewModel
    {
        public int QuestionNumber { get; set; }  // رقم السؤال الحالي
        public int TotalQuestions { get; set; }  // إجمالي عدد الأسئلة
        public string QuestionText { get; set; } // نص السؤال
        public List<string> Choices { get; set; } // الخيارات المتاحة للسؤال
        public int TimerSeconds { get; set; } // الوقت المحدد لحل السؤال
        public string CorrectAnswer { get; set; } // الإجابة الصحيحة
        public string QuestionImageUrl { get; set; } // رابط الصورة (إذا وُجد)
    }
}
