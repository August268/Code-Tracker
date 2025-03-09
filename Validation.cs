using System.Globalization;

namespace Code_Tracker
{
    public class Validation
    {
        public bool ValidateDateTime(string dateTime)
        {
            if (!DateTime.TryParseExact(dateTime, "dd-MM-yyyy HH:mm:ss", new CultureInfo("en-US"), DateTimeStyles.None, out _))
            {
                return false;
            }
            
            return true;
        }
    }
}