using Microsoft.EntityFrameworkCore;
using Asp.Versioning;
using Serilog;
using Ergon.Contexts;
using Ergon.Interfaces;
using Ergon.Repositories;
using Ergon.Middlewares;
using Ergon.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Contexts
builder.Services.AddDbContext<ErgonContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region ApiVersioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
});
#endregion

#region Mappers
builder.Services.AddAutoMapper(typeof(Program));
#endregion

#region Loggers
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();
#endregion



#region Repositories
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<ILeaveRepository, LeaveRepository>();
builder.Services.AddScoped<IPayrollRepository, PayrollRepository>();
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
#endregion

#region Services
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IDesignationService, DesignationService>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<ILeaveTypeService, LeaveTypeService>();
builder.Services.AddScoped<ISalaryStructureService, SalaryStructureService>();
builder.Services.AddScoped<ISalaryComponentService, SalaryComponentService>();
builder.Services.AddScoped<ITaxSlabService, TaxSlabService>();
builder.Services.AddScoped<IPublicHolidayService, PublicHolidayService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IStateService, StateService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<ILeaveEntitlementService, LeaveEntitlementService>();
builder.Services.AddScoped<ILeaveEntitlementComponentService, LeaveEntitlementComponentService>();
#endregion


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
