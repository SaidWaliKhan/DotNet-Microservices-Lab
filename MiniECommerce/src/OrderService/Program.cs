using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderService.Data;
using OrderService.Handlers;
using OrderService.Middleware;
using OrderService.Services.HttpService;
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
    builder.Services.AddTransient<AuthHeaderForwardingHandler>();


    // register db 
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

    .AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(500 * retryAttempt)));

    //dd the jwt 
    var jwtSigningKey = builder.Configuration["Jwt:SigningKey"] ??
    throw new InvalidCastException("Jwt is missing in configuration");

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>

       {
           options.MapInboundClaims = false;
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuer = true,
               ValidateAudience = true,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,
               ValidIssuer = builder.Configuration["Jwt:Issuer"],
               ValidAudience = builder.Configuration["Jwt:Audience"],
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey)),
               ClockSkew = TimeSpan.FromSeconds(30)
           };
       });

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

    app.UseAuthentication();
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