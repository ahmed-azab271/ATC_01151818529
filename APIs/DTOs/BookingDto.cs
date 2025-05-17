using Core.Entites;

namespace APIs.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public DateTime BookedAt { get; set; } = DateTime.UtcNow;
        public string AppUserId { get; set; }
    }
}
