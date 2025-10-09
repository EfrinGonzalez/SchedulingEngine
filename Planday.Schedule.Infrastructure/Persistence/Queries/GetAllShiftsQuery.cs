using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Application.Interfaces.Infrastructure.Persistence.Queries;
using Planday.Schedule.Application.Interfaces.Infrastructure.Providers;
using Planday.Schedule.Domain.Entities;

using System.Globalization;

namespace Planday.Schedule.Infrastructure.Persistence.Queries
{
    public class GetAllShiftsQuery(IConnectionStringProvider connectionStringProvider) : IGetAllShiftsQuery
    {
        private readonly IConnectionStringProvider _connectionStringProvider = connectionStringProvider;

        public async Task<IReadOnlyCollection<Shift>> GetAllShiftsAsync(CancellationToken cancellationToken = default)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());
            await sqlConnection.OpenAsync(cancellationToken);

            var command = new CommandDefinition(Sql, cancellationToken: cancellationToken);
            var shiftDtos = await sqlConnection.QueryAsync<ShiftDto>(command);

            var shifts = shiftDtos.Select(x =>
                new Shift(
                    x.Id,
                    x.EmployeeId,
                    DateTime.Parse(x.Start, CultureInfo.InvariantCulture),
                    DateTime.Parse(x.End, CultureInfo.InvariantCulture)));           

            return shifts.ToList();
        }

        private const string Sql = @"SELECT Id, EmployeeId, Start, End FROM Shift;";
        private record ShiftDto(long Id, long? EmployeeId, string Start, string End);
    }    
}

