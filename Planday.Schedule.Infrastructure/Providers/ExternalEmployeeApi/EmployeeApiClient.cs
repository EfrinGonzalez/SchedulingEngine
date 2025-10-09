
using Microsoft.Extensions.Options;
using Planday.Schedule.Application.Interfaces.Infrastructure.Providers;
using System.Text.Json;


namespace Planday.Schedule.Infrastructure.Providers.ExternalEmployeeApi
{
    public class EmployeeApiClient : IEmployeeInfoService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public EmployeeApiClient(HttpClient httpClient, IOptions<EmployeeApiOptions> options)
        {
            _httpClient = httpClient;

            if (_httpClient.BaseAddress is null && !string.IsNullOrWhiteSpace(options.Value.BaseUrl))
            {
                _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
            }

            if (!string.IsNullOrWhiteSpace(options.Value.ApiKey) &&
                !_httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", options.Value.ApiKey);
            }
        }

        public async Task<string?> GetEmployeeEmailAsync(long employeeId, CancellationToken cancellationToken = default)
        {
            try
            {
                using var response = await _httpClient.GetAsync($"employee/{employeeId}", cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var employee = await JsonSerializer.DeserializeAsync<EmployeeResponse>(stream, _serializerOptions, cancellationToken);
                return employee?.Email;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        private sealed record EmployeeResponse(string? Email);
    }
}
