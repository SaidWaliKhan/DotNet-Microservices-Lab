using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Services;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// register db here
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=order.db"));

    
// Register ProductServiceClient as a "typed HttpClient".
builder.Services.AddHttpClient<ProductServiceClient>(client =>
{
    var baseUrl = builder.Configuration["Services:ProductService"]
        ?? "http://localhost:5001";
    client.BaseAddress = new Uri(baseUrl);
})

// This is the resilience part: if ProductService is briefly unavailable
// (e.g. still starting up in Docker), retry 3 times before giving up.
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(500 * retryAttempt)));

var app = builder.Build();



// 2. we create auto create the db if not created ..
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    db.Database.EnsureCreated();
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();