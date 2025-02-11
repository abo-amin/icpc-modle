
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
//using icpc_modle.Data;
using icpc_modle.Models;
using Google;
using icpc_modle.Models;

public class ExamController : Controller
{
    private readonly AppDbContext _context;

    public ExamController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Exam()
    {
        string userEmail = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToAction("VerifyEmail");
        }

        var allowEntry = _context.AllowedEmails.FirstOrDefault(a => a.Email == userEmail);
        if (allowEntry == null)
        {
            return Forbid("ليس لديك صلاحية لدخول الامتحان.");
        }

        // التحقق إذا كان المستخدم قد أجاب على الامتحان من قبل
        var userAnswered = _context.ExamResults.Any(u => u.Email == userEmail);
        if (userAnswered)
        {
            return RedirectToAction("Finish");  // أو صفحة أخرى تُعلمه أنه لا يمكنه الدخول مرة أخرى
        }

        DateTime currentTime = DateTime.Now;
        if (currentTime < allowEntry.StartTime || (allowEntry.EndTime.HasValue && currentTime > allowEntry.EndTime.Value))
        {
            return Forbid("ليس لديك صلاحية لدخول الامتحان.");
        }

        int questionNumber = HttpContext.Session.GetInt32("CurrentQuestion") ?? 0;
        var questions = _context.Questions.OrderBy(q => q.Id).ToList();

        if (questionNumber >= questions.Count)
        {
            return RedirectToAction("Finish");
        }

        var question = questions[questionNumber];

        var viewModel = new ExamViewModel
        {
            QuestionNumber = questionNumber,
            QuestionText = question.Text,
            Choices = question.Choices.Split(';').ToList(),
            QuestionImageUrl = question.ImageUrl,
            TimerSeconds = question.TimerSeconds
        };

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult Exam(int questionNumber, string SelectedAnswer)
    {
        string userEmail = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToAction("VerifyEmail");
        }

        var allowEntry = _context.AllowedEmails.FirstOrDefault(a => a.Email == userEmail);
        if (allowEntry == null)
        {
            return Forbid("ليس لديك صلاحية لدخول الامتحان.");
        }


        int currentQuestion = HttpContext.Session.GetInt32("CurrentQuestion") ?? 0;
        if (questionNumber != currentQuestion)
        {
            return RedirectToAction("Exam");
        }

        var questions = _context.Questions.OrderBy(q => q.Id).ToList();
        if (questionNumber >= questions.Count)
        {
            return RedirectToAction("Finish");
        }

        var question = questions[questionNumber];
        var userAnswer = new UserAnswer
        {
            QuestionId = question.Id,
            Answer = SelectedAnswer ?? "-",
            Email = userEmail,
            StartTime = DateTime.Now
        };

        _context.UserAnswers.Add(userAnswer);
        _context.SaveChanges();

        HttpContext.Session.SetInt32("CurrentQuestion", questionNumber + 1);
        return RedirectToAction("Exam");
    }

    public IActionResult Finish()
    {
        string userEmail = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToAction("VerifyEmail");
        }

        var allowEntry = _context.AllowedEmails.FirstOrDefault(a => a.Email == userEmail);
        if (allowEntry == null)
        {
            return Forbid("ليس لديك صلاحية لدخول الامتحان.");
        }

        DateTime currentTime = DateTime.Now;
        if (allowEntry.EndTime.HasValue && currentTime > allowEntry.EndTime.Value)
        {
            ViewBag.ExamEnded = true;
        }

        var existingResult = _context.ExamResults.FirstOrDefault(e => e.Email == userEmail);
        if (existingResult != null)
        {
            return View(existingResult);
        }

        int totalQuestions = _context.Questions.Count();
        var userAnswers = _context.UserAnswers.Where(u => u.Email == userEmail).ToArray();

        int correctAnswers = userAnswers.Count(u =>
            _context.Questions.Any(q => q.Id == u.QuestionId && (q.CorrectChoice ?? "") == (u.Answer ?? ""))
        );

        double percentage = (totalQuestions > 0) ? ((double)correctAnswers / totalQuestions) * 100 : 0;
        string status = percentage >= 70 ? "Accepted" : "Rejected";

        var examResult = new ExamResult
        {
            Email = userEmail,
            CorrectAnswers = correctAnswers,
            TotalQuestions = totalQuestions,
            Percentages = percentage,
            Status = status
        };

        _context.ExamResults.Add(examResult);
        _context.SaveChanges();

        return View();
    }
}

