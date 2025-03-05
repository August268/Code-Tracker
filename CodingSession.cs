using System.Globalization;

namespace Code_Tracker
{
    public class CodingSession
    {
        public int Id { get; set; }
        public required string StartTime { get; set; }
        public required string EndTime { get; set; }
        public double Duration { get; set; }
    }
}