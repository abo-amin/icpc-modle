using Microsoft.EntityFrameworkCore;

namespace icpc_modle.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Question> Questions { get; set; }
        public DbSet<AllowedEmail> AllowedEmails { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
    }
}
