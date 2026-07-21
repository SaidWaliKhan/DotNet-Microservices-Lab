using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Middleware;
using Serilog;



// we add the Serilog before building because to catch the error before building the application.

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "ProductService")
    .WriteTo.Console(
        outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level:u3}] " +
            "({Service}) " +
            "[{CorrelationId}] " +
            "{Message:lj}{NewLine}{Exception}")
        .CreateLogger();

try
{
    Log.Information("Starting up Product Service");

    var builder = WebApplication.CreateBuilder(args);

    // replace the existing ILogger 
    builder.Host.UseSerilog();

    // 1. Register services here
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Register our DbContext, pointing at a local SQLite file.
    // Each microservice gets its OWN database file - that's the whole point.
    builder.Services.AddDbContext<ProductDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source = product.db"));


    var app = builder.Build();

    // 2. we create auto create the db if not created ..
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
        db.Database.Migrate();
    }


    app.UseMiddleware<CorrelationIdMiddleware>();

    app.UseSerilogRequestLogging();
    // now add the Excepptionhandler middleware here

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // 3. HTTP request pipeline 
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
   
    app.UseAuthorization();
    app.MapControllers();

    app.Run();

}
catch (Exception ex)

{
    Log.Fatal(ex, "Product Service Terminated Unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}