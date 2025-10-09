using Planday.Schedule.Application.Interfaces.Infrastructure.Persistence.Commands;
using Planday.Schedule.Application.Interfaces.Infrastructure.Persistence.Queries;
using Planday.Schedule.Application.Interfaces.Infrastructure.Providers;
using Planday.Schedule.Application.Interfaces.Services;
using Planday.Schedule.Application.Services;
using Planday.Schedule.Infrastructure.Persistence.Commands;
using Planday.Schedule.Infrastructure.Persistence.Queries;
using Planday.Schedule.Infrastructure.Providers.ExternalEmployeeApi;
using Planday.Schedule.Infrastructure.Providers.SQLiteConnector;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConnectionStringProvider>(new ConnectionStringProvider(builder.Configuration.GetConnectionString("Database")));

builder.Services.Configure<EmployeeApiOptions>(builder.Configuration.GetSection("EmployeeApi"));
builder.Services.AddHttpClient<IEmployeeInfoService, EmployeeApiClient>();

builder.Services.AddScoped<IShiftReadService, ShiftReadService>();
builder.Services.AddScoped<IShiftWriteService, ShiftWriteService>();
builder.Services.AddScoped<IGetShiftByIdQuery, GetShiftByIdQuery>();
builder.Services.AddScoped<IGetAllShiftsQuery, GetAllShiftsQuery>();
builder.Services.AddScoped<ICreateOpenShiftCommand, CreateOpenShiftCommand>();
builder.Services.AddScoped<IGetEmployeeByIdQuery, GetEmployeeByIdQuery>();
builder.Services.AddScoped<IEmployeeHasOverlappingShiftQuery, EmployeeHasOverlappingShiftQuery>();
builder.Services.AddScoped<IAssignShiftCommand, AssignShiftCommand>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
