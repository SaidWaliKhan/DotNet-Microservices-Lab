using Microsoft.EntityFrameworkCore;
using Serilog;
using UserService.Data;
using UserService.Middleware;


Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "UserService")
    .WriteTo.Console(
        outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level:u3}] " +
            "({Service}) " +
            "[{CorrelationId}] " +
            "{Message:lj}{NewLine}{Exception}")
        .CreateLogger();

try
{
    Log.Information("Starting up User Service");
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDbContext<UserDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=user.db"));

    var app = builder.Build();


    // 2. we create auto create the db if not created ..
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        db.Database.Migrate();
    }


    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseSerilogRequestLogging();
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}

catch (Exception ex)
{
    Log.Fatal(ex, "User Service Terminated Unexpedtedly");
}

finally
{
    Log.CloseAndFlush();
}