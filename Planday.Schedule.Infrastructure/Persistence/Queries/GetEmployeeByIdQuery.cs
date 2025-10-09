using Dapper;
using Microsoft.Data.Sqlite;
using Planday.Schedule.Application.Interfaces.Infrastructure.Persistence.Queries;
using Planday.Schedule.Application.Interfaces.Infrastructure.Providers;
using Planday.Schedule.Domain.Entities;

namespace Planday.Schedule.Infrastructure.Persistence.Queries
{
    public class GetEmployeeByIdQuery(IConnectionStringProvider connectionStringProvider) : IGetEmployeeByIdQuery
    {
        private readonly IConnectionStringProvider _connectionStringProvider = connectionStringProvider;

        public async Task<Employee?> GetEmployeeByIdAsync(long employeeId, CancellationToken cancellationToken = default)
        {
            await using var sqlConnection = new SqliteConnection(_connectionStringProvider.GetConnectionString());
            await sqlConnection.OpenAsync(cancellationToken);

            var command = new CommandDefinition(Sql, new { Id = employeeId }, cancellationToken: cancellationToken);
            var dto = await sqlConnection.QuerySingleOrDefaultAsync<EmployeeDto>(command);

            return dto is null ? null : new Employee(dto.Id, dto.Name);
        }

        private const string Sql = @"SELECT Id, Name FROM Employee WHERE Id = @Id;";
        private sealed record EmployeeDto(long Id, string Name);
    }
}
