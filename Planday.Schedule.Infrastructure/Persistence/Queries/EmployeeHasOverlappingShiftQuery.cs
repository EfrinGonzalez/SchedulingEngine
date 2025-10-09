using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Application.Interfaces.Infrastructure.Persistence.Queries;
using Planday.Schedule.Application.Interfaces.Infrastructure.Providers;

namespace Planday.Schedule.Infrastructure.Persistence.Queries
{
    public class EmployeeHasOverlappingShiftQuery(IConnectionStringProvider connectionStringProvider) : IEmployeeHasOverlappingShiftQuery
    {
        private readonly IConnectionStringProvider _connectionStringProvider = connectionStringProvider;

        public async Task<bool> HasOverlappingShiftAsync(long employeeId, DateTime start, DateTime end, long excludeShiftId, CancellationToken cancellationToken = default)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());
            await sqlConnection.OpenAsync(cancellationToken);

            var parameters = new
            {
                EmployeeId = employeeId,
                Start = start,
                End = end,
                ExcludeShiftId = excludeShiftId
            };

            var command = new CommandDefinition(Sql, parameters, cancellationToken: cancellationToken);
            var count = await sqlConnection.ExecuteScalarAsync<long>(command);

            return count > 0;
        }

        private const string Sql = @"SELECT COUNT(1)
                                     FROM Shift
                                     WHERE EmployeeId = @EmployeeId
                                      AND Id != @ExcludeShiftId
                                      AND NOT (@End <= Start OR @Start >= End);";
    }
}
