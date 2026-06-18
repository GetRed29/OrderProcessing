using OrderProcessing.API.Endpoints;
using OrderProcessing.Application.Repositories;
using OrderProcessing.Application.Validation;
using OrderProcessing.Infrastructure.Persistence.Sqlite;

SQLitePCL.Batteries.Init();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=orderprocessing.db";

builder.Services.AddScoped<IOrderRepository>(sp => new SqliteOrderRepository(connectionString));

builder.Services.AddSingleton<IDatabaseInitializer>(sp => new SqliteDatabaseInitializer(connectionString));

builder.Services.AddScoped<IStockRepository>(sp => new SqliteStockRepository(connectionString));

builder.Services.AddScoped<IOrderValidationHandler>(sp =>
{
    var ageHandler = new AgeVerificationHandler();
    var fraudHandler = new FraudDetectionHandler();
    var priceHandler = new PriceValidationHandler();

    var stockRepository = sp.GetRequiredService<IStockRepository>();
    var stockHandler = new StockValidationHandler(stockRepository);

    ageHandler.SetNext(fraudHandler).SetNext(priceHandler).SetNext(stockHandler);

    return ageHandler;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
    initializer.Initialize();
}

if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
}

app.UseCors("AllowReactFrontend");

app.UseStaticFiles();

app.MapOrderEndpoints();

app.Run();