using FastEndpoints;

namespace BeamOS.DirectStiffnessMethod.Api;

public class MyEndpoint : Endpoint<MyRequest, MyResponse>
{
    public override void Configure()
    {
        this.Post("/api/user/create");
        this.AllowAnonymous();
    }

    public override async Task HandleAsync(MyRequest req, CancellationToken ct)
    {
        await this.SendAsync(new()
        {
            FullName = req.FirstName + " " + req.LastName,
            IsOver18 = req.Age > 18
        }, cancellation: ct);
    }
}

public class MyRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public int Age { get; set; }
}

public class MyResponse
{
    public required string FullName { get; set; }
    public bool IsOver18 { get; set; }
}
