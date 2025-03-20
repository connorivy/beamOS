namespace BeamOs.Identity;

public interface IUserIdProvider
{
    public int? UserId { get; }
}

public class UserIdProvider : IUserIdProvider
{
    public int? UserId { get; set; }
}
