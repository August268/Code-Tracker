using System.Configuration;
using System.Data.SQLite;
using System.Globalization;
using System.Data;
using Dapper;
using Microsoft.VisualBasic;


namespace Code_Tracker
{
    public class CodingSessionController
    {
        readonly string _connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");

        public void CreateSession(string startTime, string endTime)
        {
            var duration = CalculateDuration(startTime, endTime);

            using IDbConnection connection = new SQLiteConnection(_connectionString);

            var query = $"INSERT INTO CodingSessions (StartTime, EndTime, Duration) VALUES ('{startTime}', '{endTime}', '{duration}')";

            connection.Execute(query);
        }

        public void DeleteSession(string id)
        {
            using IDbConnection connection = new SQLiteConnection(_connectionString);

            var query = $"DELETE FROM CodingSessions WHERE Id={id}";

            connection.Execute(query);
        }

        public List<CodingSession> GetSessions()
        {
            using IDbConnection connection = new SQLiteConnection(_connectionString);

            string query = "SELECT * FROM CodingSessions";
            
            List<CodingSession> data = connection.Query<CodingSession>(query).ToList();

            return data;
        }

        public void UpdateSession(int id, string newStartTime, string newEndTime)
        {
            var newDuration = CalculateDuration(newStartTime, newEndTime);

            using IDbConnection connection = new SQLiteConnection(_connectionString);

            string query = $"UPDATE CodingSessions SET StartTime='{newStartTime}', EndTime='{newEndTime}', Duration='{newDuration}' WHERE Id='{id}'";

            connection.Execute(query);
        }

        private double CalculateDuration(string startTime, string endTime)
        {
            DateTime startDateTime = DateTime.ParseExact(startTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime endDateTime = DateTime.ParseExact(endTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            return (endDateTime - startDateTime).TotalHours;
        }
    }
}