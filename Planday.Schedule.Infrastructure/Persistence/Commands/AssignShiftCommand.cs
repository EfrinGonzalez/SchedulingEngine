using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Application.Interfaces.Infrastructure.Persistence.Commands;
using Planday.Schedule.Application.Interfaces.Infrastructure.Providers;

namespace Planday.Schedule.Infrastructure.Persistence.Commands
{
    public class AssignShiftCommand(IConnectionStringProvider connectionStringProvider) : IAssignShiftCommand
    {
        private readonly IConnectionStringProvider _connectionStringProvider = connectionStringProvider;

        public async Task AssignShiftAsync(long shiftId, long employeeId, CancellationToken cancellationToken = default)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());
            await sqlConnection.OpenAsync(cancellationToken);

            var command = new CommandDefinition(Sql, new { ShiftId = shiftId, EmployeeId = employeeId }, cancellationToken: cancellationToken);
            var affected = await sqlConnection.ExecuteAsync(command);

            if (affected == 0)
            {
                throw new InvalidOperationException("No shift was updated during assignment.");
            }
        }

        private const string Sql = @"UPDATE Shift SET EmployeeId = @EmployeeId WHERE Id = @ShiftId;";
    }
}
