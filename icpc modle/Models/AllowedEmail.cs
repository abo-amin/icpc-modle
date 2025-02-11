using System.ComponentModel.DataAnnotations;

namespace icpc_modle.Models
{
    public class AllowedEmail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        // Property to calculate the total time difference
        public  TimeSpan TotalTime
        {
            get
            {
                if (EndTime.HasValue)
                {
                    return EndTime.Value - StartTime;
                }
                else
                {
                    return TimeSpan.Zero; // Return zero if EndTime is null
                }
            }
        }
    }
}
