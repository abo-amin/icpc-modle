public class Question
{
    public int Id { get; set; }
    public string Text { get; set; }
    public string Choices { get; set; } // جميع الخيارات في عمود واحد مفصولة بـ ";"
    public string CorrectChoice { get; set; } // الإجابة الصحيحة
    public string? ImageUrl { get; set; } // رابط الصورة
    public int TimerSeconds { get; set; } // المدة الزمنية لكل سؤال
}
