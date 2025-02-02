using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace icpc_modle.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }  // نص السؤال

        [Required]
        public string Choices { get; set; } // تخزين الخيارات كـ "اختيار1;اختيار2;اختيار3;اختيار4"

        [Required]
        public string CorrectChoice { get; set; } // الإجابة الصحيحة

        public string ImageUrl { get; set; } // رابط الصورة (إن وجدت)

        [Required]
        public int TimerSeconds { get; set; } // عدد الثواني المتاحة للسؤال

        [NotMapped]
        public List<string> ChoicesList
        {
            get => Choices.Split(';').ToList(); // عند القراءة: تحويل النص إلى قائمة
            set => Choices = string.Join(";", value); // عند الكتابة: تحويل القائمة إلى نص مفصول بـ ";"
        }
    }
}

