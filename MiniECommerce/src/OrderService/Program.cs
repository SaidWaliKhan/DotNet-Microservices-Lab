using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Handlers;
using OrderService.Middleware;
using OrderService.Services.HttpServices;
using Polly;
using Polly.Extensions.Http;
using Serilog;

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
    Log.Information("Starting up Order Service");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // needs a correlationdelagatorHandler can read the current http request(httpContext) like correlationId from inside a class that is not controller
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddTransient<CorrelationIdDelegatingHandler>();

    // register db here
    builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source = order.db"));


    // Register ProductServiceClient as a "typed HttpClient".
    builder.Services.AddHttpClient<ProductServiceClient>(client =>
    {
        var baseUrl = builder.Configuration["Services:ProductService"]
            ?? "http://localhost:5001";
        client.BaseAddress = new Uri(baseUrl);
        client.Timeout = TimeSpan.FromSeconds(5);
    })

    // we add the correalation befoe the retry policy so the corrrelation header is set on every attempt
    .AddHttpMessageHandler<CorrelationIdDelegatingHandler>()

    // This is the resilience part: if ProductService is briefly unavailable
    // (e.g. still starting up in Docker), retry 3 times before giving up.

    .AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 3, durationOfBreak: TimeSpan.FromSeconds(30)));
    

    // for user 
    builder.Services.AddHttpClient<UserServiceClient>(client =>
    {
        var baseUrl = builder.Configuration["Services:UserService"]
            ?? "http://localhost:5003";
        client.BaseAddress = new Uri(baseUrl);
    })

    .AddHttpMessageHandler<CorrelationIdDelegatingHandler>()

    .AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(500 * retryAttempt)));



    var app = builder.Build();


    //  we create auto create the db if not created ..
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        db.Database.Migrate();
    }

    // now add the Exceptionhandler here
    app.UseMiddleware<OrderService.Middleware.ExceptionHandlingMiddleware>();

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

    catch(Exception ex)
{
    Log.Fatal(ex,"Order Service Terminated Unexpectedly");
}

finally
{

    Log.CloseAndFlush();
}