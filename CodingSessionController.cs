using System.Configuration;
using System.Data.SQLite;
using System.Globalization;
using System.Data;
using Dapper;


namespace Code_Tracker
{
    public class CodingSessionController
    {
        readonly string _connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");

        public void CreateSession(string startTime, string endTime)
        {
            var duration = CalculateDuration(startTime, endTime);

            using IDbConnection connection = new SQLiteConnection(_connectionString);

            var sql = $"INSERT INTO CodingSessions (StartTime, EndTime, Duration) VALUES ('{startTime}', '{endTime}', '{duration}')";

            connection.Execute(sql);
        }

        public void DeleteSession(int id)
        {
            using IDbConnection connection = new SQLiteConnection(_connectionString);

            var sql = $"DELETE FROM CodingSessions WHERE id='{id}";

            connection.Execute(sql);
        }

        public List<CodingSession> GetAllSessions()
        {
            using IDbConnection connection = new SQLiteConnection(_connectionString);

            string query = "SELECT * FROM CodingSessions";

            return [.. connection.Query<CodingSession>(query)];
        }

        private double CalculateDuration(string startTime, string endTime)
        {
            DateTime startDateTime = DateTime.ParseExact(startTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime endDateTime = DateTime.ParseExact(endTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            return (endDateTime - startDateTime).TotalHours;
        }
    }
}