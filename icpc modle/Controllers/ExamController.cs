using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using icpc_modle.Models;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using icpc_modle.Helpers;

namespace icpc_modle.Controllers
{
    public class ExamController : Controller
    {
        private static readonly List<Question> Questions = new List<Question>
        {
            new Question 
            { Id = 1,
                Text = "What is the next number in this sequence?\r\n",
                ChoicesList = new List<string> { "A) 18\r\n", "B) 32\r\n", " C) 24\r\n", "D) 64\r\n\r\n" }
            , TimerSeconds = 15, CorrectChoice = "القاهرة", ImageUrl = null },
            new Question { Id = 2, Text = "ما هو أكبر كوكب؟", ChoicesList = new List<string> { "الأرض", "المشتري", "زحل", "المريخ" }, TimerSeconds = 20, CorrectChoice = "المشتري", ImageUrl = "https://th.bing.com/th/id/OIP.3E2geOWOR5HfwHNJxlKaRgHaFS?rs=1&pid=ImgDetMain" },
            new Question { Id = 3, Text = "ما هو أسرع حيوان بري؟", ChoicesList = new List<string> { "الفهد", "الأسد", "الذئب", "الفيل" }, TimerSeconds = 10, CorrectChoice = "الفهد", ImageUrl = string.Empty }
        };

        private readonly string emailsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "AllowedEmails.xlsx");
        private readonly string resultsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ExamResults.xlsx");

        [HttpGet]
        public IActionResult Exam(int questionNumber = 1)
        {
            string userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail) || !IsEmailAllowed(userEmail))
            {
                return RedirectToAction("Index", "Home");
            }

            if (questionNumber > Questions.Count)
            {
                return RedirectToAction("Finish");
            }

            var question = Questions[questionNumber - 1];
            return View(new ExamViewModel
            {
                QuestionNumber = questionNumber,
                TotalQuestions = Questions.Count,
                QuestionText = question.Text,
                Choices = question.ChoicesList,
                TimerSeconds = question.TimerSeconds,
                QuestionImageUrl = question.ImageUrl,
                CorrectAnswer = question.CorrectChoice
            });
        }

        [HttpPost]
        public IActionResult Exam(int questionNumber, string SelectedAnswer)
        {
            string userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail) || !IsEmailAllowed(userEmail))
            {
                return RedirectToAction("Index", "Home");
            }

            var attempt = HttpContext.Session.Get<ExamAttempt>("ExamResults") ?? new ExamAttempt { Email = userEmail };
            var currentQuestion = Questions[questionNumber - 1];

            attempt.Answers.Add(new AnswerRecord
            {
                QuestionId = currentQuestion.Id,
                QuestionText = currentQuestion.Text,
                SelectedAnswer = SelectedAnswer,
                CorrectAnswer = currentQuestion.CorrectChoice,
                QuestionImageUrl = currentQuestion.ImageUrl,
                IsCorrect = SelectedAnswer == currentQuestion.CorrectChoice
            });

            HttpContext.Session.Set("ExamResults", attempt);
            return RedirectToAction("Exam", new { questionNumber = questionNumber + 1 });
        }

        [HttpGet]
        public IActionResult Finish()
        {
            var attempt = HttpContext.Session.Get<ExamAttempt>("ExamResults");
            if (attempt == null) return RedirectToAction("Index", "Home");

            SaveAttemptToExcel(attempt);
            HttpContext.Session.Clear();
            return View();
        }

        private void SaveAttemptToExcel(ExamAttempt attempt)
        {
            string resultsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ExamResults.xlsx");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            FileInfo file = new FileInfo(resultsFilePath);

            // إذا كان الملف موجودًا، نقوم بتحميله وإذا لم يكن موجودًا ننشئه.
            using (var package = file.Exists ? new ExcelPackage(file) : new ExcelPackage())
            {
                // ورقة نتائج الإجابات
                var worksheet = package.Workbook.Worksheets.FirstOrDefault() ?? package.Workbook.Worksheets.Add("Results");

                if (worksheet.Dimension == null)
                {
                    worksheet.Cells["A1"].Value = "البريد الإلكتروني";
                    worksheet.Cells["B1"].Value = "رقم السؤال";
                    worksheet.Cells["C1"].Value = "السؤال";
                    worksheet.Cells["D1"].Value = "الإجابة المختارة";
                    worksheet.Cells["E1"].Value = "الإجابة الصحيحة";
                    worksheet.Cells["F1"].Value = "رابط الصورة";
                    worksheet.Cells["G1"].Value = "صحيحة؟";
                }

                int newRow = worksheet.Dimension?.Rows + 1 ?? 2;

                foreach (var answer in attempt.Answers)
                {
                    worksheet.Cells[$"A{newRow}"].Value = attempt.Email;
                    worksheet.Cells[$"B{newRow}"].Value = answer.QuestionId;
                    worksheet.Cells[$"C{newRow}"].Value = answer.QuestionText;
                    worksheet.Cells[$"D{newRow}"].Value = answer.SelectedAnswer;
                    worksheet.Cells[$"E{newRow}"].Value = answer.CorrectAnswer;
                    worksheet.Cells[$"F{newRow}"].Value = answer.QuestionImageUrl;
                    worksheet.Cells[$"G{newRow}"].Value = answer.IsCorrect ? "نعم" : "لا";
                    newRow++;
                }

                // حساب النسبة المئوية للإجابات الصحيحة
                double correctAnswersPercentage = (double)attempt.Answers.Count(a => a.IsCorrect) / attempt.Answers.Count * 100;
                string result = correctAnswersPercentage >= 70 ? "مقبول" : "غير مقبول";

                // إضافة ورقة جديدة لنتيجة القبول
                var resultWorksheet = package.Workbook.Worksheets.FirstOrDefault(w => w.Name == "Result")
                                      ?? package.Workbook.Worksheets.Add("Result");

                resultWorksheet.Cells["A1"].Value = "البريد الإلكتروني";
                resultWorksheet.Cells["B1"].Value = "النسبة المئوية للإجابات الصحيحة";
                resultWorksheet.Cells["C1"].Value = "النتيجة";

                int resultRow = resultWorksheet.Dimension?.Rows + 1 ?? 2;
                resultWorksheet.Cells[$"A{resultRow}"].Value = attempt.Email;
                resultWorksheet.Cells[$"B{resultRow}"].Value = correctAnswersPercentage;
                resultWorksheet.Cells[$"C{resultRow}"].Value = result;

                // إذا كان الملف موجودًا، نستخدم Save لتحديثه، وإذا كان غير موجود، نستخدم SaveAs لإنشاءه.
                if (file.Exists)
                {
                    package.Save(); // تحديث الملف إذا كان موجودًا
                }
                else
                {
                    package.SaveAs(file); // إنشاء الملف إذا لم يكن موجودًا
                }
            }
        }




        private bool IsEmailAllowed(string email)
        {
            if (!System.IO.File.Exists(emailsFilePath)) return false;

            using (var package = new ExcelPackage(new FileInfo(emailsFilePath)))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null) return false;

                int rowCount = worksheet.Dimension.Rows;
                for (int row = 1; row <= rowCount; row++)
                {
                    string storedEmail = worksheet.Cells[row, 1].Text.Trim();
                    if (email == storedEmail) return true;
                }
            }

            return false;
        }
    }
}
