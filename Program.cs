using System.Globalization;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* 
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
	.MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .CreateLogger();

	

builder.Host.UseSerilog(); */


var app = builder.Build();

var logger = app.Logger;

int RollDice()
{
    return Random.Shared.Next(1, 7);
}

string HandleRollDice(string? player)
{
    var result = RollDice();

    if (string.IsNullOrEmpty(player))
    {
        logger.LogInformation($"Anonymous player is rolling the dice: {result}", result.ToString());
    }
    else
    {
        logger.LogInformation("{player} is rolling the dice: {result}", player, result);
    }


    switch (result)
    {
        case < 3:
            logger.LogError($"Lower value {result}");
            break;
        case < 4:
            logger.LogCritical($"Medium value {result}");
            break;
        default:
            logger.LogWarning($"High value {result}");
            break;
    }

    return result.ToString(CultureInfo.InvariantCulture);
}

app.MapGet("/prueba/{player?}", HandleRollDice);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


