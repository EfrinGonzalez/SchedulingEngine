using System.Globalization;
using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Application.Interfaces.Infrastructure.Persistence.Queries;
using Planday.Schedule.Application.Interfaces.Infrastructure.Providers;
using Planday.Schedule.Domain.Entities;

namespace Planday.Schedule.Infrastructure.Persistence.Queries;

public class GetShiftByIdQuery(IConnectionStringProvider connectionStringProvider) : IGetShiftByIdQuery
{
    private readonly IConnectionStringProvider _connectionStringProvider = connectionStringProvider;

    public async Task<Shift?> GetShiftByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());
        await sqlConnection.OpenAsync(cancellationToken);

        var command = new CommandDefinition(Sql, new { Id = id }, cancellationToken: cancellationToken);
        var shiftDto = await sqlConnection.QuerySingleOrDefaultAsync<ShiftDto>(command);

        if (shiftDto is null)
        {
            return null;
        }

        return new Shift(
           shiftDto.Id,
           shiftDto.EmployeeId,
           DateTime.Parse(shiftDto.Start, CultureInfo.InvariantCulture),
           DateTime.Parse(shiftDto.End, CultureInfo.InvariantCulture));
    }

    private const string Sql = @"SELECT Id, EmployeeId, Start, End FROM Shift WHERE Id = @Id;";
    private record ShiftDto(long Id, long? EmployeeId, string Start, string End);
}
