using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Code_Tracker
{
    public class Validation
    {
        public bool ValidateDateTime(string dateTime)
        {
            if (!DateTime.TryParseExact(dateTime, "dd-MM-yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
            {
                return false;
            }
            
            return true;
        }
    }
}