using System;

namespace VizoMenuAPIv3.Models
{
    public class VenueHour
    {
        public Guid Id { get; set; }
        public Guid VenueId { get; set; }
        public int DayOfWeek { get; set; } // 0 = Sunday … 6 = Saturday
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }
        public bool IsClosed { get; set; }

        public Venue? Venue { get; set; }
    }
}
