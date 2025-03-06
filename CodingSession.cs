using System.Globalization;

namespace Code_Tracker
{
    public class CodingSession(int id, string startTime, string endTime, double duration)
    {
        public int Id { get; set; } = id;
        public string StartTime { get; set; } = startTime;
        public string EndTime { get; set; } = endTime;
        public double Duration { get; set; } = duration;
    }
}