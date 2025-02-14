namespace BeamOs.Identity;

public interface IUserApiUsageService
{
    public Task<ApiUsageResponse?> GetApiUsage();
}

public class ExampleUserApiUsageService : IUserApiUsageService
{
    public Task<ApiUsageResponse?> GetApiUsage()
    {
        return Task.FromResult<ApiUsageResponse?>(
            new()
            {
                TotalCalls = 142,
                TotalDurationMs = 47560,
                Breakdown = new List<UsageBreakdownResponse>
                {
                    new()
                    {
                        ActorName = "Mobile App Token",
                        NumCalls = 65,
                        TotalDurationMs = 19560,
                        IsToken = true
                    },
                    new()
                    {
                        ActorName = "Web Dashboard Token",
                        NumCalls = 45,
                        TotalDurationMs = 18200,
                        IsToken = false
                    },
                    new()
                    {
                        ActorName = "CI/CD Pipeline Token",
                        NumCalls = 32,
                        TotalDurationMs = 9800,
                        IsToken = false
                    },
                }
            }
        );
    }
}
