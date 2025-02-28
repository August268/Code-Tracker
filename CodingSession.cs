namespace Code_Tracker
{
    public class CodingSession
    {
        public int Id { get; set; }
        public required string StartTime { get; set; }
        public required string EndTime { get; set; }
        public float Duration { get; set; }

        public void CalculateDuration() {
            try
            {
                DateTime startDate = DateTime.Parse(StartTime);
                DateTime endDate = DateTime.Parse(EndTime);
            }
            catch (System.Exception)
            {
                
                throw;
            }
        }
    }
}