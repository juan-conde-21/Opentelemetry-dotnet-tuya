using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace TodoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
     private static readonly ActivitySource MyActivitySource = new("OpenTelemetry.TodoApiAppDemo.Jaeger");

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
 /* using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(
                serviceName: "TodoApiAppDemo",
                serviceVersion: "1.0.0"))
            .AddSource("OpenTelemetry.TodoApiAppDemo.Jaeger")
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter()
            .Build(); */

           // callActivity().GetAwaiter().GetResult();
     

             _logger.LogWarning("Seri Log is Working");
          // Ejemplo de información sensible (número de tarjeta de crédito)
            var creditCardNumber = "1234-5678-9012-3456";

            // No agregues información sensible directamente en los registros,
            // utiliza el enriquecimiento para evitar exponerlo.
            _logger.LogWarning("Se ha recibido una solicitud GET en el endpoint /api/example");

            // Enriquecimiento: No exponer información sensible en los registros
            _logger.LogWarning("Número de tarjeta de crédito: {@CreditCardNumber}", new { CreditCardNumber = MaskCreditCard(creditCardNumber) });

            // Otro registro enriquecido
            _logger.LogError("Registro adicional con información enriquecida");
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray(); 

        
        
    }



    private async Task callActivity(){

               using var parent = MyActivitySource.StartActivity("JaegerDemo");

        using (var client = new HttpClient())
        {
            using (var slow = MyActivitySource.StartActivity("SomethingSlow"))
            {
                await client.GetStringAsync("https://httpstat.us/200?sleep=1000");
                await client.GetStringAsync("https://httpstat.us/200?sleep=1000");
            }

            using (var fast = MyActivitySource.StartActivity("SomethingFast"))
            {
               await  client.GetStringAsync("https://httpstat.us/301");
            }
        }
    }
     private string MaskCreditCard(string creditCardNumber)
        {
            // Lógica para enmascarar el número de tarjeta de crédito
            // Ejemplo: 1234-5678-9012-3456 -> XXXX-XXXX-XXXX-3456
            return "XXXX-XXXX-XXXX-" + creditCardNumber.Substring(creditCardNumber.Length - 4);
        }
}
