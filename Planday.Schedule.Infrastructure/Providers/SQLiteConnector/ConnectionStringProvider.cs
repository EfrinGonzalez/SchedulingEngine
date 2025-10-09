using System.Text.RegularExpressions;
using Planday.Schedule.Application.Interfaces.Infrastructure.Providers;

namespace Planday.Schedule.Infrastructure.Providers.SQLiteConnector
{
    public class ConnectionStringProvider(string connectionString) : IConnectionStringProvider
    {
        private readonly string _connectionString = ProcessConnectionString(connectionString);

        public string GetConnectionString()
        {
            return _connectionString;
        }

        private static string ProcessConnectionString(string connectionString)
        {
            const string pattern = "(.*=)(.*)(;.*)";
            var match = Regex.Match(connectionString, pattern);
            return Regex.Replace(
                connectionString,
                pattern,
                $"$1{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, match.Groups[2].Value)}$3");
        }
    }    
}

