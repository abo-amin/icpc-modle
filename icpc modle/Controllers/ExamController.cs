using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using icpc_modle.Models;
using System.Linq;

namespace icpc_modle.Controllers
{
    public class ExamController : Controller
    {
        private readonly AppDbContext _context;

        public ExamController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Exam(int questionNumber = 1)
        {
            var question = _context.Questions.OrderBy(q => q.Id).Skip(questionNumber - 1).FirstOrDefault();
            if (question == null)
            {
                return RedirectToAction("Finish");
            }

            var viewModel = new icpc_modle.Models.ExamViewModel  // ✅ تأكد من استخدام المسار الكامل
            {
                QuestionNumber = questionNumber,
                QuestionText = question.Text,
                Choices = question.Choices.Split(';').ToList(),
                QuestionImageUrl = question.ImageUrl,
                TimerSeconds = question.TimerSeconds
            };

            return View(viewModel);
        }
    }
}
