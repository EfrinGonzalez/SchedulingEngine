namespace Planday.Schedule.Application.Interfaces.Infrastructure.Providers
{
    public interface IEmployeeInfoService
    {
        Task<string?> GetEmployeeEmailAsync(long employeeId, CancellationToken cancellationToken = default);
    }
}
