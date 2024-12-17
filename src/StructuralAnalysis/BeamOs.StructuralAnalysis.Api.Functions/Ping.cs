using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace BeamOs.StructuralAnalysis.Api.Functions;

public class Ping
{
    //private readonly ILogger<Ping> _logger;

    //public Ping(ILogger<Ping> logger)
    //{
    //    this._logger = logger;
    //}

    [Function(nameof(Ping))]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ping")] HttpRequest req
    )
    {
        //this._logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}
