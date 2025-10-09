using Dapper;
using Microsoft.Data.Sqlite;
using System.Globalization;
using Planday.Schedule.Domain.Entities;
using Planday.Schedule.Application.Interfaces.Infrastructure.Persistence.Commands;
using Planday.Schedule.Application.Interfaces.Infrastructure.Providers;


namespace Planday.Schedule.Infrastructure.Persistence.Commands
{
    public class CreateOpenShiftCommand(IConnectionStringProvider connectionStringProvider) : ICreateOpenShiftCommand
    {
        private readonly IConnectionStringProvider _connectionStringProvider = connectionStringProvider;

        public async Task<Shift> CreateOpenShiftAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());
            await sqlConnection.OpenAsync(cancellationToken);

            var parameters = new
            {
                Start = start.ToString(DateTimeFormat, CultureInfo.InvariantCulture),
                End = end.ToString(DateTimeFormat, CultureInfo.InvariantCulture)
            };

            var insertCommand = new CommandDefinition(InsertSql, parameters, cancellationToken: cancellationToken);
            var newId = await sqlConnection.ExecuteScalarAsync<long>(insertCommand);

            var queryCommand = new CommandDefinition(SelectSql, new { Id = newId }, cancellationToken: cancellationToken);
            var shift = await sqlConnection.QuerySingleAsync<ShiftDto>(queryCommand);

            return new Shift(
                shift.Id,
                shift.EmployeeId,
                DateTime.Parse(shift.Start, CultureInfo.InvariantCulture),
                DateTime.Parse(shift.End, CultureInfo.InvariantCulture));
        }

        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
        private const string InsertSql = @"INSERT INTO Shift (EmployeeId, Start, End)
                                           VALUES (NULL, @Start, @End);
                                           SELECT last_insert_rowid();";
        private const string SelectSql = @"SELECT Id, EmployeeId, Start, End FROM Shift WHERE Id = @Id;";
        private record ShiftDto(long Id, long? EmployeeId, string Start, string End);
    }
}
