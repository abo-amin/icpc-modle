using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Collections.Generic;
using OfficeOpenXml;
using icpc_modle.Models;
using icpc_modle.Helpers;
// ده الي بيتاعامل مع شيت ال excel

namespace icpc_modle.Controllers
{
    public class HomeController : Controller
    {
        private static readonly HashSet<string> AttemptedEmails = new HashSet<string>();

        // GET: /Home/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: /Home/Index
        [HttpPost]
        public IActionResult Index(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage = "Invalid email, Must be Gmail.";
                return View();
            }

            if (AttemptedEmails.Contains(email.ToLower()))
            {
                ViewBag.ErrorMessage = "You have already attempted the exam and cannot attempt it again.";
                return View();
            }
            if (HasAttemptedExam(email))
            {
                ViewBag.ErrorMessage = "You have already completed the exam, you cannot try again.";
                return View();
            }

            if (!IsEmailAllowed(email))
            {
                ViewBag.ErrorMessage = "You are not allowed to take the exam.";
                return View();
            }

            HttpContext.Session.SetString("UserEmail", email);

            ExamAttempt attempt = new ExamAttempt
            {
                Email = email
            };
            HttpContext.Session.Set("ExamAttempt", attempt);

            return RedirectToAction("Exam", "Exam", new { questionNumber = 1 });
        }

        private bool HasAttemptedExam(string email)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "C:\\Users\\Abo Amin\\Documents\\icpc modle\\icpc modle\\ExamResults.xlsx");
            if (!System.IO.File.Exists(filePath))
            {
                return false; 
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Result"]; // نفترض أن اسم الورقة "Attempts"
                if (worksheet == null)
                {
                    return false;
                }

                int rowCount = worksheet.Dimension.Rows;
                for (int row = 2; row <= rowCount; row++) // بدء الفحص من الصف الثاني
                {
                    string recordedEmail = worksheet.Cells[row, 1].Text.Trim(); // نفترض أن البريد في العمود الأول
                    if (recordedEmail.Equals(email, StringComparison.OrdinalIgnoreCase))
                    {
                        return true; // البريد موجود، أي أنه دخل الامتحان من قبل
                    }
                }
            }
            return false;
        }

        private bool IsEmailAllowed(string email)
        {
            // تحديد مسار ملف Excel (تأكد أن الملف موجود في مجلد المشروع)
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "C:\\Users\\Abo Amin\\Documents\\icpc modle\\icpc modle\\AllowedEmails.xlsx");
            if (!System.IO.File.Exists(filePath))
            {
                // إذا لم يوجد الملف، نرجع false
                return false;
            }

            // إعداد ترخيص EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                // نفترض أن ورقة العمل اسمها "Emails"
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Emails"];
                if (worksheet == null)
                {
                    return false;
                }

                // نبدأ من الصف 2 (نفترض أن الصف الأول يحتوي على عنوان العمود)
                int rowCount = worksheet.Dimension.Rows;
                for (int row = 2; row <= rowCount; row++)
                {
                    string allowedEmail = worksheet.Cells[row, 1].Text.Trim();
                    if (allowedEmail.Equals(email, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

       
        public static void AddAttemptedEmail(string email)
        {
            AttemptedEmails.Add(email.ToLower());
        }
    }
}
