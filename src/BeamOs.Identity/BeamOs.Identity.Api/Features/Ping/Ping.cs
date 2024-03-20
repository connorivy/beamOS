using BeamOS.Common.Api;

namespace BeamOs.Identity.Api.Features.Ping;


//public class Ping : BeamOsFastEndpoint<string, string>
//{
//    public override void Configure()
//    {
//        this.Get("/ping");
//        this.AllowAnonymous();
//    }

//    public override Task<string> ExecuteAsync(string? req, CancellationToken ct)
//    {
//        return Task.FromResult(
//            $"{this.HttpContext.Request.Scheme}://{this.HttpContext.Request.Host}"
//        );
//    }
//}
